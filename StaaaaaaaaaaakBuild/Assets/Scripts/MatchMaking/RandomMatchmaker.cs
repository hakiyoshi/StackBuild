using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using NetworkSystem;
using StackBuild.Game;
using UniRx;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

namespace StackBuild.MatchMaking
{
    public sealed class RandomMatchmaker : SyncClientsNetworkBehaviour, IRandomMatchmaker
    {
        [SerializeField] private LobbyManager lobby;
        [SerializeField] private RelayManager relay;
        [SerializeField] private LobbyOption lobbyOption;
        [SerializeField] private PlayerOption playerOption;

        private readonly AsyncSubject<Unit> succeedMatchmaking = new ();
        public IObservable<Unit> SucceedMatchmaking => succeedMatchmaking;
        private readonly AsyncSubject<Unit> allClientReady = new();
        public IObservable<Unit> AllClientReady => allClientReady;

        private CancellationTokenSource cts;

        private void Awake()
        {
            succeedMatchmaking.AddTo(this);
            allClientReady.AddTo(this);
        }

        public override void OnDestroy()
        {
            StopRandomMatchmaking().Forget();
        }

        public async UniTask StartRandomMatchmaking()
        {
            InitializeCancellationTokenSource();

            try
            {
                await NetworkSystemManager.NetworkInitAsync();
            }
            catch (Exception ex)
            {
                succeedMatchmaking.OnError(ex);
                throw;
            }

            try
            {
                await NetworkSystemManager.ClientQuickAsync(lobby, relay, cts.Token);
            }
            catch (Exception)
            {
                try
                {
                    await NetworkSystemManager.CreateRoomAsync(false, lobby, relay, lobbyOption, playerOption,
                        cts.Token);
                }
                catch (Exception ex)
                {
                    succeedMatchmaking.OnError(ex);
                    throw;
                }
            }

            try
            {
                await UniTask.WaitUntil(() => IsSpawned, cancellationToken: cts.Token);
                await UniTask.WaitUntil(() => connectedClientCount >= 2, cancellationToken: cts.Token);

                succeedMatchmaking.OnNext(Unit.Default);
                succeedMatchmaking.OnCompleted();
            }
            catch (Exception ex)
            {
                succeedMatchmaking.OnError(ex);
                throw;
            }
        }

        public async UniTask StopRandomMatchmaking()
        {
            FinalizeCancellationTokenSource();
            ResetParameters();

            await NetworkSystemManager.NetworkExit(lobby, relay);
        }

        public async UniTask SceneChangeReady()
        {
            SendReadyServerRpc();

            try
            {
                await UniTask.WaitUntil(() => readyClientCount >= 2, cancellationToken: cts.Token);

                allClientReady.OnNext(Unit.Default);
                allClientReady.OnCompleted();
            }
            catch (Exception ex)
            {
                allClientReady.OnError(ex);
                throw;
            }
        }

        private void InitializeCancellationTokenSource()
        {
            if (cts != null) return;

            cts = new();
            cts.AddTo(this);
        }

        private void FinalizeCancellationTokenSource()
        {
            if (cts == null) return;

            cts.Cancel();
            cts.Dispose();
            cts = null;
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
        }
    }

    public interface IRandomMatchmaker
    {
        public IObservable<Unit> SucceedMatchmaking { get; }
        public UniTask StartRandomMatchmaking();
        public UniTask StopRandomMatchmaking();
    }
}
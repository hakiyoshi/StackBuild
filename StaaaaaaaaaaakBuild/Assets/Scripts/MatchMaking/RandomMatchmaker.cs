using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using NetworkSystem;
using UniRx;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;

namespace StackBuild.MatchMaking
{
    public sealed class RandomMatchmaker : MonoBehaviour, IRandomMatchmaker
    {
        [SerializeField] private LobbyManager lobby;
        [SerializeField] private RelayManager relay;
        [SerializeField] private LobbyOption lobbyOption;
        [SerializeField] private PlayerOption playerOption;

        private readonly AsyncSubject<Unit> succeedMatchmaking = new AsyncSubject<Unit>();
        public IObservable<Unit> SucceedMatchmaking => succeedMatchmaking;

        private CancellationTokenSource cts;

        private void Awake()
        {
            succeedMatchmaking.AddTo(this);
        }

        private void OnDestroy()
        {
            StopRandomMatchmaking();
        }

        public async UniTask StartRandomMatchmaking()
        {
            InitializeCancellationTokenSource();

            try
            {
                await NetworkSystemManager.NetworkInitAsync();
            }
            catch (Exception)
            {
                // ignored
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
                catch (Exception)
                {
                    // ignored
                }
            }
            finally
            {
                try
                {
                    await UniTask.WaitUntil(() => NetworkManager.Singleton.ConnectedClientsList.Count >= 2,
                        cancellationToken: cts.Token);

                    SuccessRandomMatchmaking();
                }
                catch (Exception)
                {
                    // ignored
                }
            }
        }

        public void StopRandomMatchmaking()
        {
            FinalizeCancellationTokenSource();
        }

        private void SuccessRandomMatchmaking()
        {
            succeedMatchmaking.OnNext(Unit.Default);
            succeedMatchmaking.OnCompleted();
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
    }

    public interface IRandomMatchmaker
    {
        public IObservable<Unit> SucceedMatchmaking { get; }
        public UniTask StartRandomMatchmaking();
        public void StopRandomMatchmaking();
    }
}
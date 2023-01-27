using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using NetworkSystem;
using UniRx;
using UnityEngine;

namespace StackBuild.MatchMaking
{
    public sealed class RandomMatchmaker : SyncClientsNetworkBehaviour, IRandomMatchmaker
    {
        [SerializeField] private LobbyManager lobby;
        [SerializeField] private RelayManager relay;
        [SerializeField] private LobbyOption lobbyOption;
        [SerializeField] private PlayerOption playerOption;

        private CancellationTokenSource cts;

        public override void OnDestroy()
        {
            FinalizeCancellationTokenSource();
        }

        public async UniTask StartRandomMatchmaking()
        {
            InitializeCancellationTokenSource();

            await NetworkSystemManager.NetworkInitAsync();

            try
            {
                await NetworkSystemManager.ClientQuickAsync(lobby, relay, cts.Token);
            }
            catch (Exception)
            {
                await NetworkSystemManager.CreateRoomAsync(false, lobby, relay, lobbyOption, playerOption,
                    cts.Token);
            }

            await UniTask.WaitUntil(() => IsSpawned, cancellationToken: cts.Token);
            await UniTask.WaitUntil(() => connectedClientCount >= 2, cancellationToken: cts.Token);
        }

        public async UniTask StopRandomMatchmaking()
        {
            FinalizeCancellationTokenSource();

            await NetworkSystemManager.NetworkExit(lobby, relay);
            ResetParameters();
        }

        public async UniTask SceneChangeReady()
        {
            SendReadyServerRpc();

            await UniTask.WaitUntil(() => readyClientCount >= 2, cancellationToken: cts.Token);
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
        public UniTask StartRandomMatchmaking();
        public UniTask StopRandomMatchmaking();
    }
}
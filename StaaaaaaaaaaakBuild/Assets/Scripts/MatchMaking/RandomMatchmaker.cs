using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using NetworkSystem;
using StackBuild.Game;
using UniRx;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
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
            StopRandomMatchmaking().Forget();
        }

        public async UniTask StartRandomMatchmaking()
        {
            InitializeCancellationTokenSource();

            try
            {
                await NetworkSystemManager.NetworkInitAsync();
            }
            catch (OperationCanceledException ex)
            {
                Debug.Log("Canceled RandomMatchmaking");
                throw;
            }
            catch (Exception ex)
            {
                throw;
            }

            try
            {
                await NetworkSystemManager.ClientQuickAsync(lobby, relay, cts.Token);
            }
            catch (OperationCanceledException ex)
            {
                Debug.Log("Canceled RandomMatchmaking");
                throw;
            }
            catch (Exception)
            {
                try
                {
                    await NetworkSystemManager.CreateRoomAsync(false, lobby, relay, lobbyOption, playerOption,
                        cts.Token);
                }
                catch (OperationCanceledException ex)
                {
                    Debug.Log("Canceled RandomMatchmaking");
                    throw;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }

            try
            {
                await UniTask.WaitUntil(() => IsSpawned, cancellationToken: cts.Token);
                await UniTask.WaitUntil(() => connectedClientCount >= 2, cancellationToken: cts.Token);
            }
            catch (OperationCanceledException ex)
            {
                Debug.Log("Canceled RandomMatchmaking");
                throw;
            }
            catch (Exception ex)
            {
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
            }
            catch (OperationCanceledException ex)
            {
                Debug.Log("Canceled SceneChangeReady");
                throw;
            }
            catch (Exception ex)
            {
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
        public UniTask StartRandomMatchmaking();
        public UniTask StopRandomMatchmaking();
    }
}
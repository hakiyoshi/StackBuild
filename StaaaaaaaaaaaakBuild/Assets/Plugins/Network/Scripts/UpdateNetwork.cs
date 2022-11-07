using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using UnityEngine;

namespace NetworkSystem
{
    public class UpdateNetwork : MonoBehaviour
    {
        public LobbyManager lobby;
        public RelayManager relay;

        private CancellationTokenSource token = null;

        private void Start()
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += x =>
            {
                //Relayから自分が落ちた場合ロビーからも抜ける
                if(x == NetworkManager.Singleton.LocalClientId)
                    NetworkSystemManager.NetworkExit(lobby, relay).Forget();
            };

            relay.OnRelaySetting.Where(x => x == RelayManager.SettingEvent.Join).Subscribe(_ =>
            {
                CancelToken();

                token = new CancellationTokenSource();
                CheckRelayConnectionStatus(token.Token).Forget();
            }).AddTo(this);
            
            relay.OnRelaySetting.Where(x => x == RelayManager.SettingEvent.Exit).Subscribe(_ =>
            {
                CancelToken();
            }).AddTo(this);
        }

        private void OnDestroy()
        {
            CancelToken();
        }

        private async UniTask CheckRelayConnectionStatus(CancellationToken token)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(5), cancellationToken: token);
            while (true)
            {
                token.ThrowIfCancellationRequested();
                if (!NetworkManager.Singleton.IsConnectedClient)
                {
                    NetworkSystemManager.NetworkExit(lobby,relay).Forget();
                    return;
                }
                
                await UniTask.Yield(cancellationToken: token);
            }
        }

        private void CancelToken()
        {
            if (token == null)
                return;
                
            token.Cancel();
            token.Dispose();
            token = null;
        }

        private void OnApplicationQuit()
        {
            //ホストの場合
            NetworkSystemManager.NetworkExit(lobby, relay).Forget();
        }
    }
}

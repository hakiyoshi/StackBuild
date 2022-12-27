using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UniRx.Triggers;
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

        private IDisposable disposable = null;

        private void Start()
        {
            Observable.FromEvent<ulong>(
                x => NetworkManager.Singleton.OnClientDisconnectCallback += x,
                x => NetworkManager.Singleton.OnClientDisconnectCallback -= x).Subscribe(x =>
            {
                //Relayから自分が落ちた場合ロビーからも抜ける
                if (x == NetworkManager.Singleton.LocalClientId)
                    NetworkSystemManager.NetworkExit(lobby, relay).Forget();
            }).AddTo(this);

            Observable.FromEvent(
                x => NetworkManager.Singleton.OnTransportFailure += x,
                x => NetworkManager.Singleton.OnTransportFailure -= x).Subscribe(_ =>
            {
                //Transportに失敗したら呼ばれるらしい
                NetworkSystemManager.NetworkExit(lobby, relay).Forget();
            }).AddTo(this);

            relay.OnRelaySetting.Where(x => x == RelayManager.SettingEvent.Join).
                Delay(TimeSpan.FromSeconds(5)).
                Subscribe(_ =>
                {
                    disposable = this.UpdateAsObservable().Subscribe(_ =>
                    {
                        if (NetworkManager.Singleton.IsConnectedClient)
                            return;

                        NetworkSystemManager.NetworkExit(lobby, relay).Forget();
                    });
                }).AddTo(this);

            relay.OnRelaySetting.Where(x => x == RelayManager.SettingEvent.Exit).Subscribe(_ =>
            {
                if(lobby.Status == LobbyManager.LobbyStatus.Client)
                    disposable.Dispose();
            }).AddTo(this);
        }

        private void OnDestroy()
        {
            if(lobby.Status == LobbyManager.LobbyStatus.Client)
                disposable.Dispose();
        }

        private void OnApplicationQuit()
        {
            //ホストの場合
            NetworkSystemManager.NetworkExit(lobby, relay).Forget();
        }
    }
}

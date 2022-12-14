using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay.Models;
using UnityEngine;

namespace NetworkSystem
{
    public class NetworkSystemManager : MonoBehaviour
    {
        public static GameObject NetworkManagerObject { get; private set; }
        public static NetworkSystem.NetworkSystemSceneManager SceneManagerObject { get; private set; }

        public static async UniTask NetworkInitAsync()
        {
            if (UnityServices.State != ServicesInitializationState.Uninitialized)
                return;

            //最初にUnityServicesを初期化する
            await UnityServices.InitializeAsync();

            try
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                Debug.Log("匿名サインインに成功");
                Debug.Log($"プレイヤーID: {AuthenticationService.Instance.PlayerId}");

                // ロビーマネージャーオブジェクトを作成
                if (NetworkManagerObject == null)
                {
                    NetworkManagerObject = Instantiate(Resources.Load("NetworkManager") as GameObject);
                    DontDestroyOnLoad(NetworkManagerObject);
                }

                if (SceneManagerObject == null)
                {
                    SceneManagerObject = Instantiate(Resources.Load("NetworkSceneManager") as GameObject)
                        .GetComponent<NetworkSystemSceneManager>();
                    DontDestroyOnLoad(SceneManagerObject);
                }
            }
            catch (AuthenticationException ex)
            {
                Debug.LogException(ex);
                throw;
            }
            catch (RequestFailedException ex)
            {
                Debug.LogException(ex);
                throw;
            }
        }

        public static async UniTask NetworkExit(LobbyManager lobby, RelayManager relay)
        {
            relay.RelayExit();
            await lobby.LobbyExit();
        }

        public static async UniTask CreateRoomAsync(
            bool isServer,                  // false: Host
            LobbyManager lobbyManager,
            RelayManager relayManager,
            LobbyOption lobbyOption,
            PlayerOption playerOption,
            CancellationToken token,
            Region targetRegion = null
        )
        {
            //Relayを先に準備
            await relayManager.CreateAllocationAsync(LobbyManager.DefaultMaxPlayer, isServer, targetRegion);
            token.ThrowIfCancellationRequested();

            //LobbyOptionにJoinCodeをセット
            lobbyOption.UpdateRelayJoinCode(relayManager.JoinCode);

            //ロビー作成
            await lobbyManager.CreateLobbyAsync("Test Lobby", LobbyManager.DefaultMaxPlayer, false, lobbyOption, playerOption);
        }

        public static async UniTask ClientQuickAsync(LobbyManager lobbyManager, RelayManager relayManager, CancellationToken token)
        {
            //クイック入室
            await lobbyManager.QuickJoinAsync(LobbyManager.DefaultMaxPlayer);
            token.ThrowIfCancellationRequested();

            //Relay用のIDを取得
            var lobbyData = lobbyManager.lobbyInfo.Data[LobbyOption.KeyNameRelayJoinCode];
            await JoinAllocationAsync(lobbyData.Value, relayManager, token);
        }

        public static async UniTask ClientCodeAsync(LobbyManager lobbyManager, RelayManager relayManager, string lobbyCode, CancellationToken token)
        {
            //コードを指定して入室
            await lobbyManager.JoinLobbybyCodeAsync(lobbyCode);
            token.ThrowIfCancellationRequested();

            //Relay用のIDを取得
            var lobbyData = lobbyManager.lobbyInfo.Data[LobbyOption.KeyNameRelayJoinCode];
            await JoinAllocationAsync(lobbyData.Value, relayManager, token);
        }

        public static async UniTask JoinAllocationAsync(string lobbyId, RelayManager relayManager, CancellationToken token)
        {
            try
            {
                await relayManager.JoinAllocationAsync(lobbyId);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
        }
    }
}

using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using NetworkSystem;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace NetworkSystem
{
    public class RoomManager : MonoBehaviour
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        [SerializeField] private LobbyManager lobby;
        [SerializeField] private RelayManager relay;
        [SerializeField] private LobbyOption lobbyOption;
        [SerializeField] private PlayerOption playerOption;

        private string LobbyCode = "";
        private Rect windowRect = new Rect(0f, 0f, 200f, 100f);

        [SerializeField] private SceneAsset LoadScene;
        private void OnValidate()
        {
            if (LoadScene != null)
                loadSceneName = LoadScene.name;
            else
                loadSceneName = "";
        }

        [SerializeField] private string loadSceneName;

        [SerializeField] private bool StartNetworkIsServer = false;
        [SerializeField] private bool ConnectToNetworkAtStart = false;


        [SerializeField] private bool DrawGui = true;


        private void Awake()
        {
            NetworkConnectionAtStartAsync(this.GetCancellationTokenOnDestroy()).Forget();
        }

        private void OnGUI()
        {
            if (!DrawGui)
                return;

            windowRect = GUILayout.Window(0, windowRect, NetworkGUIWindow, "Network");
        }

        void NetworkGUIWindow(int windowId)
        {
            const int fontSize = 25;

            if (GUILayout.Button("Server"))
            {
                NetworkSystemManager.CreateRoomAsync(true, lobby, relay, lobbyOption, playerOption, this.GetCancellationTokenOnDestroy()).Forget();
            }

            if (GUILayout.Button("Host"))
            {
                NetworkSystemManager.CreateRoomAsync(false, lobby, relay, lobbyOption, playerOption, this.GetCancellationTokenOnDestroy()).Forget();
            }

            if (GUILayout.Button("QuickJoin"))
            {
                NetworkSystemManager.ClientQuickAsync(lobby, relay, this.GetCancellationTokenOnDestroy()).Forget();
            }

            GUILayout.Space(5);
            LobbyCode = GUILayout.TextField(LobbyCode, 25);
            if (GUILayout.Button("LobbyCodeJoin"))
            {
                NetworkSystemManager.ClientCodeAsync(lobby, relay, LobbyCode, this.GetCancellationTokenOnDestroy()).Forget();
            }

            if (GUILayout.Button("LobbyExit"))
            {
                NetworkSystemManager.NetworkExit(lobby, relay).Forget();
            }

            GUILayout.Space(10);
            loadSceneName = GUILayout.TextField(loadSceneName, 25);
            if (GUILayout.Button("NextScene") && !string.IsNullOrEmpty(loadSceneName))
            {
                NetworkSystemSceneManager.LoadScene(loadSceneName);
            }

            if (lobby.lobbyInfo != null)
            {
                GUILayout.Label("ロビー名: " + lobby.lobbyInfo.Name);
                GUILayout.Label("ロビーコード: " + lobby.lobbyInfo.LobbyCode);

                GUILayout.Label("ロビープレイヤー数: " + lobby.lobbyInfo.Players.Count);
                if (NetworkManager.Singleton.IsServer)
                    GUILayout.Label("リレープレイヤー数: " + NetworkManager.Singleton.ConnectedClients.Count);

                string playerId = lobby.lobbyInfo.Players.Aggregate("", (current, player) => current + (player.Id + "\n"));
                GUILayout.Label("プレイヤーID\n" + playerId);

                if (lobby.lobbyInfo.Data != null)
                {
                    foreach (var option in lobby.lobbyInfo.Data)
                    {
                        GUILayout.Label(
                            $"key: {option.Key}\nvalue: {option.Value.Value}\n" +
                            $"visibility: {option.Value.Visibility}" +
                            $"index: {option.Value.Index}");
                    }
                }
            }

            GUI.DragWindow();
        }

        private async UniTask NetworkConnectionAtStartAsync(CancellationToken token)
        {
            await NetworkSystemManager.NetworkInitAsync();
            token.ThrowIfCancellationRequested();

            if (!ConnectToNetworkAtStart || NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsClient)
                return;

            await NetworkSystemManager.CreateRoomAsync(StartNetworkIsServer, lobby, relay, lobbyOption, playerOption,
                this.GetCancellationTokenOnDestroy());
            token.ThrowIfCancellationRequested();
        }

#endif
    }

}
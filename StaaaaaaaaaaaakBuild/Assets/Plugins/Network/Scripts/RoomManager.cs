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
        [SerializeField] private LobbyManager lobby;
        [SerializeField] private RelayManager relay;
        [SerializeField] private LobbyOption lobbyOption;
        [SerializeField] private PlayerOption playerOption;
        
        private string LobbyCode = "";

#if UNITY_EDITOR
        [SerializeField] private SceneAsset LoadScene;
        private void OnValidate()
        {
            if (LoadScene != null)
                loadSceneName = LoadScene.name;
            else
                loadSceneName = "";
        }
#endif
        [SerializeField] private string loadSceneName;
        
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
            
            NetworkGUIWindow(0);
        }

        void NetworkGUIWindow(int windowId)
        {
            const int fontSize = 25;
            
            var button = GUI.skin.button;
            button.fontSize = fontSize;

            var textField = GUI.skin.textField;
            textField.fontSize = fontSize;

            var label = GUI.skin.label;
            label.fontSize = fontSize;
            
            GUILayout.BeginVertical(GUI.skin.window);
            
            if (GUILayout.Button("CreateLobby", button))
            {
                NetworkSystemManager.HostAsync(lobby, relay, lobbyOption, playerOption, this.GetCancellationTokenOnDestroy()).Forget();
            }
            
            if (GUILayout.Button("QuickJoin", button))
            {
                NetworkSystemManager.ClientQuickAsync(lobby, relay, this.GetCancellationTokenOnDestroy()).Forget();
            }

            GUILayout.Space(5);
            LobbyCode = GUILayout.TextField(LobbyCode, 25, textField);
            if (GUILayout.Button("LobbyCodeJoin", button))
            {
                NetworkSystemManager.ClientCodeAsync(lobby, relay, LobbyCode, this.GetCancellationTokenOnDestroy()).Forget();
            }

            if (GUILayout.Button("LobbyExit", button))
            {
                NetworkSystemManager.NetworkExit(lobby, relay).Forget();
            }

            GUILayout.Space(10);
            loadSceneName = GUILayout.TextField(loadSceneName, 25, textField);
            if (GUILayout.Button("NextScene", button) && !string.IsNullOrEmpty(loadSceneName))
            {
                NetworkSystemSceneManager.LoadScene(loadSceneName);
            }

            if (lobby.lobbyInfo != null)
            {
                GUILayout.Label("ロビー名: " + lobby.lobbyInfo.Name, label);
                GUILayout.Label("ロビーコード: " + lobby.lobbyInfo.LobbyCode, label);

                GUILayout.Label("ロビープレイヤー数: " + lobby.lobbyInfo.Players.Count, label);
                if (NetworkManager.Singleton.IsServer)
                    GUILayout.Label("リレープレイヤー数: " + NetworkManager.Singleton.ConnectedClients.Count, label);
                
                string playerId = lobby.lobbyInfo.Players.Aggregate("", (current, player) => current + (player.Id + "\n"));
                GUILayout.Label("プレイヤーID\n" + playerId, label);

                if (lobby.lobbyInfo.Data != null)
                {
                    foreach (var option in lobby.lobbyInfo.Data)
                    {
                        GUILayout.Label(
                            $"key: {option.Key}\nvalue: {option.Value.Value}\n" + 
                            $"visibility: {option.Value.Visibility}" + 
                            $"index: {option.Value.Index}", label);
                    }
                }
            }
            
            GUILayout.EndVertical();
        }

        private async UniTask NetworkConnectionAtStartAsync(CancellationToken token)
        {
            await NetworkSystemManager.NetworkInitAsync();
            token.ThrowIfCancellationRequested();
            
            if (!ConnectToNetworkAtStart || NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsClient)
                return;

            await NetworkSystemManager.HostAsync(lobby, relay, lobbyOption, playerOption,
                this.GetCancellationTokenOnDestroy());
            token.ThrowIfCancellationRequested();
        }
    }

}
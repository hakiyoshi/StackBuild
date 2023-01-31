using System;
using Cysharp.Threading.Tasks;
using NetworkSystem;
using StackBuild.Scene.Title;
using StackBuild.UI;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace StackBuild.Game
{
    public class DisconnectionNetwork : MonoBehaviour
    {
        [SerializeField] private RelayManager relay;
        [SerializeField] private LobbyManager lobby;
        [SerializeField] private CanvasGroup loadingScreenCanvas;

        private bool startNetwork = false;
        private bool sceneChangeNow = false;

        private void Start()
        {
            if (NetworkManager.Singleton != null)
            {
                startNetwork = NetworkManager.Singleton.IsClient;
                NetworkManager.Singleton.OnClientDisconnectCallback += DisconnectClient;
            }
        }

        private void Update()
        {
            if (!sceneChangeNow && startNetwork && lobby.Status == LobbyManager.LobbyStatus.NonPerticipation)
            {
                LoadMainMenu(false);
            }
        }

        private void DisconnectClient(ulong obj)
        {
            LoadMainMenu(false);
        }

        private void LoadMainMenu(bool characterSelect)
        {
            sceneChangeNow = true;
            TitleScene.MarkTitleSkip();
            if (characterSelect)
            {
                TitleScene.MarkMainMenuSkip();
            }
            ChangeScene("MainMenu").Forget();
        }

        private async UniTask ChangeScene(string sceneName)
        {
            await NetworkSystemManager.NetworkExit(lobby, relay);
            await UniTask.WaitWhile(() => NetworkManager.Singleton.ShutdownInProgress);
            if (loadingScreenCanvas.alpha == 0)
                await LoadingScreen.Instance.ShowAsync();
            await SceneManager.LoadSceneAsync(sceneName);
        }
    }
}
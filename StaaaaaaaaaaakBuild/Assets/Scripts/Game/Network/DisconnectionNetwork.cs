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

        private bool startNetwork = false;
        private bool sceneChangeNow = false;

        private void Start()
        {
            startNetwork = NetworkManager.Singleton != null && NetworkManager.Singleton.IsClient;
        }

        private void Update()
        {
            if (!sceneChangeNow && startNetwork && lobby.Status == LobbyManager.LobbyStatus.NonPerticipation)
            {
                sceneChangeNow = true;
                LoadMainMenu(false);
            }
        }

        private void LoadMainMenu(bool characterSelect)
        {
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
            await LoadingScreen.Instance.ShowAsync();
            await SceneManager.LoadSceneAsync(sceneName);
        }
    }
}
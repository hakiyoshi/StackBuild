using System;
using Cysharp.Threading.Tasks;
using NetworkSystem;
using StackBuild.Scene.Title;
using StackBuild.UI;
using UniRx;
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
        private bool hasDisconnected = false;

        private void Start()
        {
            if (NetworkManager.Singleton != null)
            {
                var networkManager = NetworkManager.Singleton;
                startNetwork = networkManager.IsClient;
                Observable.FromEvent<ulong>(
                    handler => networkManager.OnClientDisconnectCallback += handler,
                    handler => networkManager.OnClientDisconnectCallback -= handler
                ).Subscribe(_ => OnDisconnect().Forget()).AddTo(this);
            }
        }

        private async UniTaskVoid OnDisconnect()
        {
            if(hasDisconnected) return;
            hasDisconnected = true;
            await ModalSpawner.Instance.ShowMessageModal(
                "Connection Lost",
                "対戦相手が退出したか、接続が切れました。\n対戦を終了し、メインメニューに戻ります。"
            );
            LoadMainMenu(false);
        }

        private void LoadMainMenu(bool characterSelect)
        {
            hasDisconnected = true;
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
using System.Threading;
using Cysharp.Threading.Tasks;
using NetworkSystem;
using StackBuild.Game;
using StackBuild.Scene.Title;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace StackBuild.UI
{
    public class GameOverScreenButton : SyncWaitingSystem
    {
        [SerializeField] private Button button;
        [SerializeField] private bool isRematch;
        [SerializeField] private bool isCharacterSelect;
        [SerializeField] private LobbyManager lobby;
        [SerializeField] private RelayManager relay;
        [SerializeField] private EventSystem eventSystem;

        private bool waitingForSceneChange = false;
        private bool isSelect = false;
        private bool sceneChangeNow = false;

        private void Start()
        {
            button.onClick.AddListener(ClickButton);
        }

        public override void OnDestroy()
        {
            button.onClick.RemoveListener(ClickButton);
        }

        private void ClickButton()
        {
            isSelect = true;

            eventSystem.enabled = false;
            if (isRematch)
            {
                LoadGameScene();
            }
            else
            {
                LoadMainMenu(isCharacterSelect);
            }
        }

        private void LoadGameScene()
        {
            if (IsSpawned)
            {
                NetworkChangeScene("Game", this.GetCancellationTokenOnDestroy()).Forget();
            }
            else
            {
                ChangeScene("Game").Forget();
            }
        }

        private void LoadMainMenu(bool characterSelect)
        {
            TitleScene.MarkTitleSkip();
            if (characterSelect)
            {
                TitleScene.MarkMainMenuSkip();
            }

            //ネットにつながってたらネットから切断されるようにする
            if (IsSpawned)
                RematchCancelServerRpc();
            else
                ChangeScene("MainMenu").Forget();

        }

        private async UniTask ChangeScene(string sceneName)
        {
            await LoadingScreen.Instance.ShowAsync();
            await NetworkSystemManager.NetworkExit(lobby, relay);
            if (NetworkManager.Singleton != null)
            {
                await UniTask.WaitWhile(() => NetworkManager.Singleton.ShutdownInProgress);
            }
            await SceneManager.LoadSceneAsync(sceneName);
        }

        private async UniTask NetworkChangeScene(string sceneName, CancellationToken token)
        {
            //フェードイン
            await LoadingScreen.Instance.ShowAsync();

            //同期待ちしてシーンチェンジ
            if (IsSpawned)
            {
                //待機状態を送信
                SendStandbyServerRpc();

                if (IsServer)
                {
                    await UniTask.WaitUntil(() => waitingForSceneChange, cancellationToken: token);
                    NetworkSystemSceneManager.LoadScene(sceneName);
                }
            }
        }

        protected override void OnSendStandby(int numWaitingToSignal)
        {
            if(IsSpawned && !IsServer)
                return;

            //人数見て全員集まってたらシーン遷移する
            if (numWaitingToSignal >= NetworkManager.Singleton.ConnectedClientsIds.Count)
            {
                waitingForSceneChange = true;
            }
        }

        [ServerRpc(RequireOwnership = false)]
        void RematchCancelServerRpc()
        {
            if(IsSpawned && !IsServer)
                return;

            RematchCancelClientRpc();
        }

        [ClientRpc()]
        void RematchCancelClientRpc()
        {
            eventSystem.enabled = false;
            RematchCancel().Forget();
        }

        async UniTask RematchCancel()
        {
            await LoadingScreen.Instance.ShowAsync();

            //選択していてかつ再戦の場合
            await NetworkSystemManager.NetworkExit(lobby, relay);
        }
    }
}
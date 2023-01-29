using Cysharp.Threading.Tasks;
using NetworkSystem;
using StackBuild.Game;
using StackBuild.Scene.Title;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace StackBuild.UI
{
    public class GameOverScreenButton : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private bool isCharacterSelect;
        [SerializeField] private LobbyManager lobby;
        [SerializeField] private RelayManager relay;

        private void Start()
        {
            button.onClick.AddListener(LoadMainMenu);
        }

        private void OnDestroy()
        {
            button.onClick.RemoveListener(LoadMainMenu);
        }

        private void LoadMainMenu()
        {
            TitleScene.MarkTitleSkip();
            if (isCharacterSelect)
            {
                TitleScene.MarkMainMenuSkip();
            }
            ChangeScene().Forget();
        }

        private async UniTask ChangeScene()
        {
            await NetworkSystemManager.NetworkExit(lobby, relay);
            await UniTask.WaitWhile(() => NetworkManager.Singleton.ShutdownInProgress);
            await LoadingScreen.Instance.ShowAsync();
            await SceneManager.LoadSceneAsync("MainMenu");
            await LoadingScreen.Instance.HideAsync();
        }
    }
}
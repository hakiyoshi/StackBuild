using Cysharp.Threading.Tasks;
using StackBuild.Game;
using StackBuild.Scene.Title;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace StackBuild.UI
{
    public class GameOverScreenButton : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private bool isCharacterSelect;

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
            if (!isCharacterSelect)
            {
                GameMode.Current = null;
            }
            TitleScene.ShouldSkipTitle = true;
            ChangeScene().Forget();
        }

        private async UniTask ChangeScene()
        {
            await LoadingScreen.Instance.ShowAsync();
            await SceneManager.LoadSceneAsync("MainMenu");
            await LoadingScreen.Instance.HideAsync();
        }
    }
}
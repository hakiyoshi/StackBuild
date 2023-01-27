using Cysharp.Threading.Tasks;
using StackBuild.Game;
using StackBuild.Scene.Title;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace StackBuild.UI
{
    public class MainMenuLoader : MonoBehaviour
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
            TitleScene.IsSkipTitle = true;
            SceneChange().Forget();
        }

        private async UniTask SceneChange()
        {
            await LoadingScreen.Instance.ShowAsync();
            await SceneManager.LoadSceneAsync("MainMenu");
            await LoadingScreen.Instance.HideAsync();
        }
    }
}
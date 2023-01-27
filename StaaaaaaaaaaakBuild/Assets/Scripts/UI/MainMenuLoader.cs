using Cysharp.Threading.Tasks;
using StackBuild.Game;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace StackBuild.UI
{
    public class MainMenuLoader : MonoBehaviour
    {
        [SerializeField] private bool isCharacterSelect;
        [SerializeField] private Button button;
        [SerializeField] private GameMode onlineMatchMode;
        [SerializeField] private GameMode localMatchMode;

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
            if (NetworkManager.Singleton != null &&
                (NetworkManager.Singleton.IsClient || NetworkManager.Singleton.IsServer))
            {
                MainMenuLoadSetting.SelectMode = onlineMatchMode;
            }
            else
            {
                MainMenuLoadSetting.SelectMode = localMatchMode;
            }

            if (!isCharacterSelect) MainMenuLoadSetting.SelectMode = null;

            MainMenuLoadSetting.IsSkipTitle = true;
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
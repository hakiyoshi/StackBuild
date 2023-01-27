using Cysharp.Threading.Tasks;
using StackBuild.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace StackBuild
{
    public class InitialScene : MonoBehaviour
    {

        private void Start()
        {
            OnStartAsync().Forget();
        }

        private async UniTaskVoid OnStartAsync()
        {
            await LoadingScreen.Instance.ShowAsync();
            await SceneManager.LoadSceneAsync("MainMenu");
            await LoadingScreen.Instance.HideAsync();
        }

    }
}

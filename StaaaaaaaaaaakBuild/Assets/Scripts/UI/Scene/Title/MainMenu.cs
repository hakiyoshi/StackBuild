using Cysharp.Threading.Tasks;
using UnityEngine;

namespace StackBuild.UI.Scene.Title
{

    public class MainMenu : MonoBehaviour
    {

        [SerializeField] private CanvasGroup container;
        [SerializeField] private TitleMenuStaggerDisplay staggerDisplay;
        [SerializeField] private TitleMenuButton[] buttons;

        public async UniTask ShowAsync()
        {
            container.interactable = true;

            await staggerDisplay.Display();
        }

        public void Hide()
        {
            container.interactable = false;
        }

    }

}

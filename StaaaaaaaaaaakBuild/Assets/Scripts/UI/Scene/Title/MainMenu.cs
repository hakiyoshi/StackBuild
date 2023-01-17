using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace StackBuild.UI.Scene.Title
{

    public class MainMenu : MonoBehaviour
    {

        [SerializeField] private CanvasGroup container;
        [SerializeField] private TitleMenuStaggerDisplay staggerDisplay;
        [SerializeField] private Button buttonOnlineMatch;
        [SerializeField] private Button buttonLocalMatch;
        [SerializeField] private Button buttonTutorial;
        [SerializeField] private Button buttonSettings;
        [SerializeField] private Button buttonBack;

        public Button.ButtonClickedEvent OnOnlineMatchClick => buttonOnlineMatch.onClick;
        public Button.ButtonClickedEvent OnLocalMatchClick => buttonLocalMatch.onClick;
        public Button.ButtonClickedEvent OnTutorialClick => buttonTutorial.onClick;
        public Button.ButtonClickedEvent OnSettingsClick => buttonSettings.onClick;
        public Button.ButtonClickedEvent OnBackClick => buttonBack.onClick;

        public async UniTask ShowAsync()
        {
            container.interactable = true;

            EventSystem.current.SetSelectedGameObject(buttonOnlineMatch.gameObject);

            await staggerDisplay.Display();
        }

        public void Hide()
        {
            container.interactable = false;
            staggerDisplay.Hide();
        }

    }

}

using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace StackBuild.Scene.Title
{

    public class MainMenuScreen : TitleSceneScreen
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

        public override async UniTask ShowAsync()
        {
            container.interactable = true;
            container.alpha = 1;

            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(buttonOnlineMatch.gameObject);

            await staggerDisplay.Display();
        }

        public override async UniTask HideAsync()
        {
            container.interactable = false;
            await container.DOFade(0, 0.07f);
            staggerDisplay.Hide();
        }

    }

}

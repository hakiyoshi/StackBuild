using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace StackBuild.Scene.Title
{

    public class SettingsScreen : TitleSceneScreen
    {

        [SerializeField] private CanvasGroup container;
        [SerializeField] private TitleMenuStaggerDisplay staggerDisplay;
        [SerializeField] private Button buttonBack;

        public Button.ButtonClickedEvent OnBackClick => buttonBack.onClick;

        public override async UniTask ShowAsync()
        {
            container.interactable = true;
            container.blocksRaycasts = true;
            container.alpha = 1;

            await staggerDisplay.Display();
        }

        public override async UniTask HideAsync()
        {
            container.interactable = false;
            container.blocksRaycasts = false;
            await container.DOFade(0, 0.07f);
            staggerDisplay.Hide();
        }

    }

}

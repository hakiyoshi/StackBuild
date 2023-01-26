using Cysharp.Threading.Tasks;
using DG.Tweening;
using StackBuild.UI;
using UnityEngine;
using UnityEngine.UI;

namespace StackBuild.Scene.Title
{
    public class MatchmakingScreen : TitleSceneScreen
    {

        [SerializeField] private CanvasGroup container;
        [SerializeField] private StackbuildButton cancelButton;

        public Button.ButtonClickedEvent OnCancel => cancelButton.OnClick;

        public override async UniTask ShowAsync()
        {
            cancelButton.Disabled = false;
            container.interactable = true;
            container.blocksRaycasts = true;
            await container.DOFade(1, 0.3f);
        }

        public override async UniTask HideAsync()
        {
            container.interactable = false;
            container.blocksRaycasts = false;
            await container.DOFade(0, 0.1f);
        }

    }
}
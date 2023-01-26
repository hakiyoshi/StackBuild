using Cysharp.Threading.Tasks;
using DG.Tweening;
using StackBuild.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StackBuild.Scene.Title
{
    public class MatchmakingScreen : TitleSceneScreen
    {

        [SerializeField] private CanvasGroup container;
        [SerializeField] private TMP_Text matchmakingText;
        [SerializeField] private TMP_Text cancelingText;
        [SerializeField] private StackbuildButton cancelButton;

        public Button.ButtonClickedEvent OnCancel => cancelButton.OnClick;

        public void SetCanceling()
        {
            matchmakingText.alpha = 0;
            cancelingText.alpha = 1;
            cancelButton.Disabled = true;
        }

        public override async UniTask ShowAsync()
        {
            matchmakingText.alpha = 1;
            cancelingText.alpha = 0;
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
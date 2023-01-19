using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace StackBuild.UI.Scene.Title
{
    public class CharacterSelectScreen : TitleScreenBase
    {

        [SerializeField] private CanvasGroup container;
        [SerializeField] private CharacterSelectButton[] characterButtons;
        [SerializeField] private Button backButton;
        [SerializeField] private Button readyButton;

        public Button.ButtonClickedEvent OnBackClick => backButton.onClick;
        public Button.ButtonClickedEvent OnReadyClick => readyButton.onClick;

        public override async UniTask ShowAsync()
        {
            container.interactable = true;
            container.alpha = 1;
            EventSystem.current.SetSelectedGameObject(characterButtons[0].gameObject);
        }

        public override async UniTask HideAsync()
        {
            container.interactable = false;
            await container.DOFade(0, 0.2f);
        }

    }
}

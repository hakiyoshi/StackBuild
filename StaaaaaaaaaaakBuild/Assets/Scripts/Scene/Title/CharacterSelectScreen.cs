using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using StackBuild.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UniRx;

namespace StackBuild.Scene.Title
{
    public class CharacterSelectScreen : TitleSceneScreen
    {

        [SerializeField] private CanvasGroup container;
        [SerializeField] private CharacterSelectButton[] characterButtons;
        [SerializeField] private CharacterInfoDisplay characterInfoDisplay;
        [SerializeField] private StackbuildButton backButton;
        [SerializeField] private StackbuildButton readyButton;

        private CharacterProperty characterSelected;

        public Button.ButtonClickedEvent OnBackClick => backButton.OnClick;
        public Button.ButtonClickedEvent OnReadyClick => readyButton.OnClick;

        private void Awake()
        {
            foreach (var characterButton in characterButtons)
            {
                characterButton.OnClick.Subscribe(_ => SelectCharacter(characterButton.Character)).AddTo(this);
            }
        }

        public override async UniTask ShowAsync()
        {
            container.interactable = true;
            container.alpha = 1;

            SelectCharacter(null);
            EventSystem.current.SetSelectedGameObject(characterButtons[0].gameObject);
        }

        public override async UniTask HideAsync()
        {
            container.interactable = false;
            await container.DOFade(0, 0.2f);
        }

        private void SelectCharacter(CharacterProperty character)
        {
            if (character != null && character == characterSelected) return;
            characterSelected = character;
            characterInfoDisplay.DisplayAsync(character).Forget();
            readyButton.Disabled = character == null;
            foreach (var characterButton in characterButtons)
            {
                characterButton.SetCharacterSelected(character == characterButton.Character);
            }
        }

    }
}

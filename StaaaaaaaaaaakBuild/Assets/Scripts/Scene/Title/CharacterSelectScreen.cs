using System;
using Cinemachine;
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

        [Serializable]
        private struct CharacterInfo
        {
            [SerializeField] internal Transform cameraTarget;
            [SerializeField] internal CharacterSelectButton button;
        }

        [SerializeField] private CinemachineVirtualCamera vcam;
        [SerializeField] private Transform unselectedCameraTarget;
        [SerializeField] private CanvasGroup container;
        [SerializeField] private CharacterInfo[] characters;
        [SerializeField] private CharacterInfoDisplay characterInfoDisplay;
        [SerializeField] private StackbuildButton backButton;
        [SerializeField] private StackbuildButton readyButton;

        private CharacterProperty characterSelected;

        public Button.ButtonClickedEvent OnBackClick => backButton.OnClick;
        public Button.ButtonClickedEvent OnReadyClick => readyButton.OnClick;

        private void Awake()
        {
            foreach (var character in characters)
            {
                character.button.OnClick.Subscribe(_ => SelectCharacter(character.button.Character)).AddTo(this);
            }
        }

        public override async UniTask ShowAsync()
        {
            vcam.enabled = true;
            container.interactable = true;
            container.alpha = 1;

            SelectCharacter(null);
            EventSystem.current.SetSelectedGameObject(characters[0].button.gameObject);
        }

        public override async UniTask HideAsync()
        {
            vcam.enabled = false;
            container.interactable = false;
            await container.DOFade(0, 0.2f);
        }

        private void SelectCharacter(CharacterProperty characterToSelect)
        {
            if (characterToSelect != null && characterToSelect == characterSelected) return;
            characterSelected = characterToSelect;
            if (characterToSelect == null)
            {
                vcam.Follow = unselectedCameraTarget;
            }
            characterInfoDisplay.DisplayAsync(characterToSelect).Forget();
            readyButton.Disabled = characterToSelect == null;
            foreach (var character in characters)
            {
                character.button.SetCharacterSelected(characterToSelect == character.button.Character);
                if (character.button.Character == characterSelected)
                {
                    vcam.Follow = character.cameraTarget;
                }
            }
        }

    }
}

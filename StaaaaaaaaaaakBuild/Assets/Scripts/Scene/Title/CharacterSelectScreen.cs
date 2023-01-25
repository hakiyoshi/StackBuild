using System;
using Cinemachine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using StackBuild.UI;
using TMPro;
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
            [SerializeField] internal CharacterProperty character;
            [SerializeField] internal Transform cameraTarget;
            internal CharacterSelectButton button;
        }

        [SerializeField] private CinemachineVirtualCamera vcam;
        [SerializeField] private Transform unselectedCameraTarget;
        [SerializeField] private CanvasGroup container;
        [SerializeField] private TMP_Text modeNameText;
        [SerializeField] private Transform buttonContainer;
        [SerializeField] private CharacterSelectButton buttonPrefab;
        [SerializeField] private CharacterInfo[] characters;
        [SerializeField] private CharacterInfoDisplay characterInfoDisplay;
        [SerializeField] private StackbuildButton backButton;
        [SerializeField] private StackbuildButton readyButton;

        private CharacterProperty characterSelected;

        public string ModeName
        {
            set => modeNameText.text = value;
        }
        public Button.ButtonClickedEvent OnBackClick => backButton.OnClick;
        public Button.ButtonClickedEvent OnReadyClick => readyButton.OnClick;

        private void Awake()
        {
            for (int i = 0; i < characters.Length; i++)
            {
                var i1 = i;
                characters[i].button = Instantiate(buttonPrefab, buttonContainer);
                characters[i].button.Character = characters[i].character;
                characters[i].button.OnClick.Subscribe(_ => SelectCharacter(characters[i1].character)).AddTo(this);
            }
        }

        public override async UniTask ShowAsync()
        {
            vcam.enabled = true;
            container.interactable = true;
            container.blocksRaycasts = true;
            container.alpha = 1;

            SelectCharacter(null);
            EventSystem.current.SetSelectedGameObject(characters[0].button.gameObject);
        }

        public override async UniTask HideAsync()
        {
            vcam.enabled = false;
            container.interactable = false;
            container.blocksRaycasts = false;
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
                character.button.SetCharacterSelected(characterToSelect == character.character);
                if (character.character == characterSelected)
                {
                    vcam.Follow = character.cameraTarget;
                }
            }
        }

    }
}

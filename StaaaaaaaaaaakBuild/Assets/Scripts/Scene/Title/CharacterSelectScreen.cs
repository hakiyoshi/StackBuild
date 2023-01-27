﻿using System;
using Cinemachine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using StackBuild.Extensions;
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
        [SerializeField] private CanvasGroup playerNameContainer;
        [SerializeField] private RectTransform playerNameBackground;
        [SerializeField] private TMP_Text playerNameText;
        [SerializeField] private Transform buttonContainer;
        [SerializeField] private CharacterSelectButton buttonPrefab;
        [SerializeField] private CharacterInfo[] characters;
        [SerializeField] private CharacterInfoDisplay characterInfoDisplay;
        [SerializeField] private StackbuildButton backButton;
        [SerializeField] private StackbuildButton readyButton;

        private CharacterProperty characterSelected;
        private Sequence playerTextAnimation;
        private readonly Subject<CharacterProperty> onConfirm = new();

        public string ModeName
        {
            set => modeNameText.text = value;
        }

        public string PlayerName { private get; set; }
        public bool FlipPlayerBackground { private get; set; }

        /// READYされたら選択キャラクター、戻るが押されたらnull
        public IObservable<CharacterProperty> OnConfirm => onConfirm;

        private void Awake()
        {
            for (int i = 0; i < characters.Length; i++)
            {
                var i1 = i;
                characters[i].button = Instantiate(buttonPrefab, buttonContainer);
                characters[i].button.Character = characters[i].character;
                characters[i].button.OnClick.Subscribe(_ => SelectCharacter(characters[i1].character)).AddTo(this);
            }
            readyButton.OnClick.AddListener(() => onConfirm.OnNext(characterSelected));
            backButton.OnClick.AddListener(() => onConfirm.OnNext(null));

            var playerNameContainerTransform = (RectTransform)playerNameContainer.transform;
            playerTextAnimation = DOTween.Sequence()
                .Join(playerNameContainerTransform
                    .DOSizeDelta(playerNameContainerTransform.sizeDelta.WithX(0), TitleScene.SlideDecelerationDuration)
                    .From().SetEase(TitleScene.SlideDecelerationEasing))
                .Join(playerNameText.DOFade(0, 0.2f).From().SetDelay(0.2f))
                .Join(playerNameText.rectTransform.DOScale(0.8f, 0.2f).From())
                .Append(playerNameContainerTransform.DOScale(1, 0.4f).From(2)
                    .SetEase(Ease.InOutCubic))
                .Join(playerNameContainerTransform.DOAnchorMax(new Vector2(0.5f, 0.5f), 0.4f).From()
                    .SetEase(Ease.InOutCubic))
                .Join(playerNameContainerTransform.DOAnchorMin(new Vector2(0.5f, 0.5f), 0.4f).From()
                    .SetEase(Ease.InOutCubic))
                .Join(playerNameContainerTransform.DOPivot(new Vector2(0.5f, 0.5f), 0.4f).From()
                    .SetEase(Ease.InOutCubic))
                .Join(playerNameContainerTransform.DOAnchorPos(Vector2.zero, 0.4f).From()
                    .SetEase(Ease.InOutCubic))
                .SetAutoKill(false).SetLink(gameObject).Pause();
        }

        public override async UniTask ShowAsync()
        {
            vcam.enabled = true;
            container.interactable = true;
            container.blocksRaycasts = true;
            container.alpha = 1;

            var isPlayerNameSet = PlayerName != null;
            if (isPlayerNameSet)
            {
                playerTextAnimation.Restart();
                playerNameText.text = PlayerName;
            }

            playerNameContainer.alpha = isPlayerNameSet ? 1 : 0;
            playerNameBackground.localScale = new Vector3(FlipPlayerBackground ? -1 : 1, 1, 1);

            SelectCharacter(null);
            EventSystem.current.SetSelectedGameObject(characters[0].button.gameObject);
        }

        public override async UniTask HideAsync()
        {
            playerTextAnimation.Pause();
            container.interactable = false;
            container.blocksRaycasts = false;
            await container.DOFade(0, 0.2f);
            vcam.enabled = false;
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

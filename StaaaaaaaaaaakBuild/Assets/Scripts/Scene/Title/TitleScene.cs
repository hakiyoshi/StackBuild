﻿using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace StackBuild.UI.Scene.Title
{
    public class TitleScene : MonoBehaviour
    {

        private const float MenuBackgroundAlpha = 0.5f;

        internal const float SlideAccelerationDuration = 0.35f;
        internal const Ease SlideAccelerationEasing = Ease.InCubic;
        internal const float SlideDecelerationDuration = 0.75f;
        internal const Ease SlideDecelerationEasing = Ease.OutQuint;

        [SerializeField] private Image menuBackground;
        [SerializeField] private TitleLogo logo;
        [SerializeField] private TitleScreen titleScreen;
        [SerializeField] private MainMenu mainMenu;

        private TitleScreenBase currentScreen;

        private void Start()
        {
            titleScreen.OnStartPressed.Subscribe(_ => ChangeScreen(mainMenu).Forget()).AddTo(this);
            mainMenu.OnBackClick.AddListener(() => ChangeScreen(titleScreen).Forget());

            ShowTitleAsync().Forget();
        }

        private async UniTaskVoid ShowTitleAsync()
        {
            currentScreen = titleScreen;

            var logoTransform = (RectTransform)logo.transform;
            var seq = DOTween.Sequence()
                .Append(logoTransform.DOAnchorMin(new Vector2(0.5f, 0.5f), SlideAccelerationDuration).From().SetEase(SlideAccelerationEasing))
                .Join(logoTransform.DOAnchorMax(new Vector2(0.5f, 0.5f), SlideAccelerationDuration).From().SetEase(SlideAccelerationEasing))
                .Join(logoTransform.DOPivotX(0.5f, SlideAccelerationDuration).From().SetEase(SlideAccelerationEasing))
                .Join(logoTransform.DOAnchorPosX(0, SlideAccelerationDuration).From().SetEase(SlideAccelerationEasing))
                .Pause();

            await logo.DisplayAsync();

            await seq.Play().AsyncWaitForCompletion();
            ShowBackground();
            titleScreen.ShowAsync().Forget();
        }

        private void ShowBackground()
        {
            menuBackground.DOFade(MenuBackgroundAlpha, SlideDecelerationDuration).SetEase(Ease.OutQuad);
            menuBackground.rectTransform.DOAnchorMin(new Vector2(0, 0), SlideDecelerationDuration).From().SetEase(SlideDecelerationEasing);
        }

        private async UniTaskVoid ChangeScreen(TitleScreenBase screen)
        {
            await currentScreen.HideAsync();
            currentScreen = screen;
            await currentScreen.ShowAsync();
        }

    }
}
using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using StackBuild.MatchMaking;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace StackBuild.Scene.Title
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
        [SerializeField] private MainMenuScreen mainMenuScreen;
        [SerializeField] private CharacterSelectScreen characterSelectScreen;
        [SerializeField] private MatchmakingScreen matchmakingScreen;
        [SerializeField] private MatchFoundDisplay matchFoundDisplay;
        [SerializeField] private RandomMatchmaker randomMatchmaker;
        [SerializeField] private NetworkSceneChanger sceneChanger;

        private TitleSceneScreen currentScreen;

        private void Start()
        {
            titleScreen.OnStartPressed.Subscribe(_ => ChangeScreen(mainMenuScreen).Forget()).AddTo(this);
            mainMenuScreen.OnOnlineMatchClick.AddListener(() => ChangeScreen(characterSelectScreen).Forget());
            mainMenuScreen.OnBackClick.AddListener(() => ChangeScreen(titleScreen).Forget());
            characterSelectScreen.OnBackClick.AddListener(() => ChangeScreen(mainMenuScreen).Forget());
            characterSelectScreen.OnReadyClick.AddListener(() => EnterMatchmaking().Forget());

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
            menuBackground.rectTransform.DOAnchorMin(new Vector2(0.5f, 0), SlideDecelerationDuration).From(new Vector2(0, 0)).SetEase(SlideDecelerationEasing);
        }

        private void HideBackground()
        {
            menuBackground.DOFade(0, SlideDecelerationDuration).SetEase(Ease.OutQuad);
            menuBackground.rectTransform.DOAnchorMin(new Vector2(1, 0), SlideDecelerationDuration).SetEase(SlideDecelerationEasing);
        }

        private async UniTask ChangeScreen(TitleSceneScreen screen)
        {
            if (currentScreen.ShouldShowLogo != screen.ShouldShowLogo)
            {
                logo.gameObject.SetActive(screen.ShouldShowLogo);
                if (screen.ShouldShowLogo)
                {
                    ShowBackground();
                    var logoTransform = (RectTransform)logo.transform;
                    _ = logoTransform.DOAnchorPosX(0, SlideDecelerationDuration).From(new Vector2(-200, 0)).SetEase(SlideDecelerationEasing);
                }
                else
                {
                    HideBackground();
                }
            }
            await currentScreen.HideAsync();
            currentScreen = screen;
            await currentScreen.ShowAsync();
        }

        private async UniTaskVoid EnterMatchmaking()
        {
            await ChangeScreen(matchmakingScreen);
            randomMatchmaker.StartRandomMatchmaking().Forget();
            await randomMatchmaker.SucceedMatchmaking;
            await matchFoundDisplay.DisplayAsync();
            randomMatchmaker.SceneChangeReady().Forget();
            await randomMatchmaker.AllClientReady;
            sceneChanger.SceneChange();
        }
    }
}
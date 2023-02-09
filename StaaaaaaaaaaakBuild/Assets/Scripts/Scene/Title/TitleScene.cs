using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using StackBuild.Audio;
using StackBuild.Game;
using StackBuild.MatchMaking;
using StackBuild.MenuNetwork;
using StackBuild.UI;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using InputDevice = UnityEngine.InputSystem.InputDevice;

namespace StackBuild.Scene.Title
{
    public class TitleScene : MonoBehaviour
    {
        private static bool skipTitleMarked;
        public static void MarkTitleSkip() => skipTitleMarked = true;

        private static bool skipMainMenuMarked;
        public static void MarkMainMenuSkip() => skipMainMenuMarked = true;

        private const float MenuBackgroundAlpha = 0.5f;

        internal const float SlideAccelerationDuration = 0.35f;
        internal const Ease SlideAccelerationEasing = Ease.InCubic;
        internal const float SlideDecelerationDuration = 0.75f;
        internal const Ease SlideDecelerationEasing = Ease.OutQuint;

        [SerializeField] private Image menuBackground;
        [SerializeField] private TitleLogo logo;
        [SerializeField] private TitleScreen titleScreen;
        [SerializeField] private MainMenuScreen mainMenuScreen;
        [SerializeField] private SettingsScreen settingsScreen;
        [SerializeField] private CharacterSelectScreen characterSelectScreen;
        [SerializeField] private MatchmakingScreen matchmakingScreen;
        [SerializeField] private MatchFoundDisplay matchFoundDisplay;
        [SerializeField] private RandomMatchmaker randomMatchmaker;
        [SerializeField] private NetworkSceneChanger sceneChanger;
        [SerializeField] private PlayerPropertyOperator playerPropertyOperator;
        [SerializeField] private PlayerInputProperty playerInputProperty;
        [SerializeField] private AudioChannel audioChannel;
        [SerializeField] private AudioSource titleBGM; // ちょっといろいろ諦めて直でSource使う(
        [SerializeField] private AudioSource menuBGM;
        [SerializeField] private AudioCue cueStart;
        [SerializeField] private AudioCue cueReady;
        [SerializeField] private AudioCue cueMatchFound;

        private TitleSceneScreen currentScreen;

        private void Start()
        {
            titleScreen.OnStartPressed.Subscribe(_ =>
            {
                audioChannel.Request(cueStart);
                ChangeScreen(mainMenuScreen).Forget();
            }).AddTo(this);

            mainMenuScreen.OnGameModeSelect.Subscribe(mode => OnGameModeSelectAsync(mode).Forget());
            mainMenuScreen.OnSettingsClick.AddListener(() => ChangeScreen(settingsScreen).Forget());
            mainMenuScreen.OnBackClick.AddListener(() => ChangeScreen(titleScreen).Forget());
            mainMenuScreen.OnExitClick.AddListener(() => ExitGame().Forget());

            settingsScreen.OnBackClick.AddListener(() => ChangeScreen(mainMenuScreen).Forget());

            matchmakingScreen.OnCancel.AddListener(CancelMatchmaking);

            SwitchLoadMode().Forget();
        }

        private async UniTaskVoid SwitchLoadMode()
        {
            if (skipTitleMarked)
            {
                if (skipMainMenuMarked)
                {
                    if (GameMode.Current != null)
                    {
                        currentScreen = characterSelectScreen;
                        OnGameModeSelectAsync(GameMode.Current).Forget();
                    }
                    else
                    {
                        Debug.LogWarning("GameModeの指定がないためスキップできません。");
                        ShowTitleAsync().Forget();
                    }
                }
                else
                {
                    await logo.DisplayAsync();
                    await ChangeScreen(mainMenuScreen);
                }
            }
            else
            {
                ShowTitleAsync().Forget();
            }

            skipTitleMarked = false;
            skipMainMenuMarked = false;
        }

        private async UniTaskVoid ShowTitleAsync()
        {
            currentScreen = titleScreen;

            UpdateBGM();

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

        private void UpdateBGM()
        {
            if (currentScreen == titleScreen)
            {
                titleBGM.Play();
                _ = titleBGM.DOFade(1, 1).SetEase(Ease.Linear);
                _ = menuBGM.DOFade(0, 1).SetEase(Ease.Linear).OnComplete(() => menuBGM.Pause());
            }
            else
            {
                if(!menuBGM.isPlaying)
                    menuBGM.Play();
                _ = titleBGM.DOFade(0, 0.5f).SetEase(Ease.Linear);
                _ = menuBGM.DOKill();
                _ = menuBGM.volume = 1;
            }
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
            if (currentScreen != null && currentScreen.ShouldShowLogo != screen.ShouldShowLogo)
            {
                logo.gameObject.SetActive(screen.ShouldShowLogo);
                logo.DisplayImmediately();
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
            if (currentScreen != null) await currentScreen.HideAsync();
            currentScreen = screen;
            UpdateBGM();
            await currentScreen.ShowAsync();
        }

        private async UniTaskVoid OnGameModeSelectAsync(GameMode mode)
        {
            characterSelectScreen.ModeName = mode.Name;
            characterSelectScreen.PlayerName = null;
            for (int i = 0; i < mode.PlayersToSelectCharacter.Length; i++)
            {
                var player = mode.PlayersToSelectCharacter[i];
                characterSelectScreen.PlayerName =
                    mode.PlayersToSelectCharacter.Length > 1 ? $"Player {(i + 1).ToString()}" : null;
                characterSelectScreen.FlipPlayerBackground = i % 2 == 0;
                await ChangeScreen(characterSelectScreen);
                var selectedCharacter = await characterSelectScreen.OnConfirm.First();
                if (selectedCharacter == null)
                {
                    await ChangeScreen(mainMenuScreen);
                    return;
                }
                //player.Initialize(selectedCharacter);

                var propertyList = playerPropertyOperator.characterProperties.ToList();
                int characterIndex = propertyList.FindIndex(x => x == selectedCharacter);
                playerPropertyOperator.ChangeSelectedCharacter(i, characterIndex);

                InputDevice lastUpdateDevice = null;
                var max = double.MinValue;
                foreach (var device in InputSystem.devices)
                {
                    if (device.lastUpdateTime > max)
                    {
                        lastUpdateDevice = device;
                        max = device.lastUpdateTime;
                    }
                }
                playerInputProperty.SettingPlayerDevice(i, lastUpdateDevice, mode.IsOnline);
            }

            if (mode.IsOnline)
            {
                EnterMatchmaking().Forget();
            }
            else
            {
                await LoadingScreen.Instance.ShowAsync();
                SceneManager.LoadSceneAsync("Game");
            }

            GameMode.Current = mode;
        }

        private async UniTaskVoid EnterMatchmaking()
        {
            audioChannel.Request(cueReady);
            await ChangeScreen(matchmakingScreen);
            try
            {
                await randomMatchmaker.StartRandomMatchmaking();
            }
            catch (Exception ex)
            {
                await ModalSpawner.Instance.ShowMessageModal(
                    "Matchmaking Failed",
                    "対戦相手が見つかりませんでした。時間をおいてもう一度お試しください。メインメニューに戻ります。"
                );
                await ChangeScreen(mainMenuScreen);
                Debug.LogException(ex);
                return;
            }

            audioChannel.Request(cueMatchFound);
            await matchFoundDisplay.DisplayAsync();
            menuBGM.DOFade(0, 1).SetEase(Ease.Linear);
            await randomMatchmaker.SceneChangeReady();
            await LoadingScreen.Instance.ShowAsync();
            sceneChanger.SceneChange();
        }

        private void CancelMatchmaking()
        {
            randomMatchmaker.StopRandomMatchmaking().Forget();
            matchmakingScreen.SetCanceling();
        }

        private async UniTaskVoid ExitGame()
        {
            EventSystem.current.enabled = false;
            await UniTask.WhenAll(
                LoadingScreen.Instance.ShowAsync(LoadingScreenType.Fade, Color.black),
                menuBGM.DOFade(0, 1).AsyncWaitForCompletion().AsUniTask()
            );
            Application.Quit();
        }

    }
}
using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using StackBuild.UI;
using UniRx;
using UnityEngine;

namespace StackBuild.Game
{
    public class MatchControl : MonoBehaviour
    {

        [Header("Appearance")]
        [SerializeField] private IntroDisplay introDisplay;
        [SerializeField] private float introDisplayDuration;
        [SerializeField] private CanvasGroup fade;
        [SerializeField] private float fadeIn;
        [SerializeField] private float fadeSustain;
        [SerializeField] private float fadeOut;
        [SerializeField] private HUDSlide[] huds;
        [SerializeField] private float hudDelay;
        [SerializeField] private StartDisplay startDisplay;
        [SerializeField] private float startDelay;
        [SerializeField] private TimeDisplay timeDisplay;
        [SerializeField] private int flashTimeBelow;
        [SerializeField] private FinishDisplay finishDisplay;
        [SerializeField] private ResultsDisplay resultsDisplay;
        [SerializeField] private float resultsDelay;
        [Header("Game Parameters")]
        [SerializeField] private float gameTime;
        [Header("System")]
        [SerializeField] private PlayerInputProperty playerInputProperty;

        private float timeRemaining;
        private readonly ReactiveProperty<MatchState> state = new();
        public IReadOnlyReactiveProperty<MatchState> State => state;

        private void Start()
        {
            RunMatch().Forget();
            state.AddTo(this);
        }

        private async UniTaskVoid RunMatch()
        {
            state.Value = MatchState.Starting;

            DisablePlayerMovement();
            AnimateCamera();
            introDisplay.Display();
            timeDisplay.Display(Mathf.RoundToInt(gameTime));
            await UniTask.Delay(TimeSpan.FromSeconds(introDisplayDuration));

            await fade.DOFade(1, fadeIn).From(0).SetEase(Ease.InQuad);

            introDisplay.gameObject.SetActive(false);

            await UniTask.Delay(TimeSpan.FromSeconds(fadeSustain));
            await fade.DOFade(0, fadeOut);

            await UniTask.Delay(TimeSpan.FromSeconds(hudDelay));
            foreach (var hud in huds)
            {
                hud.SlideInAsync().Forget();
            }

            await UniTask.Delay(TimeSpan.FromSeconds(startDelay));
            timeRemaining = gameTime;
            state.Value = MatchState.Ingame;
            EnablePlayerMovement();
            startDisplay.gameObject.SetActive(true);
            startDisplay.Display();
        }

        private async UniTaskVoid FinishMatch()
        {
            state.Value = MatchState.Finished;
            DisablePlayerMovement();
            foreach (var hud in huds)
            {
                hud.SlideOutAsync().Forget();
            }
            finishDisplay.gameObject.SetActive(true);
            finishDisplay.Display();

            await UniTask.Delay(TimeSpan.FromSeconds(resultsDelay));
            resultsDisplay.DisplayAsync().Forget();
        }

        private void Update()
        {
            if (state.Value != MatchState.Ingame) return;

            int lastSeconds = Mathf.CeilToInt(timeRemaining);
            timeRemaining = Mathf.Max(0, timeRemaining - Time.deltaTime);
            int seconds = Mathf.CeilToInt(timeRemaining);
            if (seconds != lastSeconds)
            {
                timeDisplay.Display(seconds, seconds <= flashTimeBelow);
            }

            if (timeRemaining == 0)
            {
                FinishMatch().Forget();
            }
        }

        private void DisablePlayerMovement()
        {
            foreach (var input in playerInputProperty.PlayerInputs)
            {
                if (input == null || input.gameObject == null) continue;
                input.gameObject.SetActive(false);
            }
        }

        private void EnablePlayerMovement()
        {
            foreach (var input in playerInputProperty.PlayerInputs)
            {
                if (input == null || input.gameObject == null) continue;
                input.gameObject.SetActive(true);
            }
        }

        private void AnimateCamera()
        {
            // TODO
        }

    }
}

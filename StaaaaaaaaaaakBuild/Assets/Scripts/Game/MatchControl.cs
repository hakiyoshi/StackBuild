using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using StackBuild.UI;
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
        [Header("Game Parameters")]
        [SerializeField] private float gameTime;

        private float timeRemaining;
        private MatchState state;

        private void Start()
        {
            RunMatch().Forget();
        }

        private async UniTaskVoid RunMatch()
        {
            state = MatchState.Starting;

            DisablePlayerMovement();
            AnimateCamera();
            introDisplay.Display();
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
            state = MatchState.Ingame;
            EnablePlayerMovement();
            startDisplay.gameObject.SetActive(true);
            startDisplay.Display();
        }

        private void FinishMatch()
        {
            state = MatchState.Finished;
            DisablePlayerMovement();
            foreach (var hud in huds)
            {
                hud.SlideOutAsync().Forget();
            }
            finishDisplay.gameObject.SetActive(true);
            finishDisplay.Display();
        }

        private void Update()
        {
            if (state != MatchState.Ingame) return;

            int lastSeconds = Mathf.CeilToInt(timeRemaining);
            timeRemaining = Mathf.Max(0, timeRemaining - Time.deltaTime);
            int seconds = Mathf.CeilToInt(timeRemaining);
            if (seconds != lastSeconds)
            {
                timeDisplay.Display(seconds, seconds <= flashTimeBelow);
            }

            if (timeRemaining == 0)
            {
                FinishMatch();
            }
        }

        private void DisablePlayerMovement()
        {
            // TODO
        }

        private void EnablePlayerMovement()
        {
            // TODO
        }

        private void AnimateCamera()
        {
            // TODO
        }

    }
}

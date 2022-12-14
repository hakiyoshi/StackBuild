using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using StackBuild.UI;
using UnityEngine;

namespace StackBuild.Game
{
    public class MatchControl : MonoBehaviour
    {

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

        private void Start()
        {
            RunMatch().Forget();
        }

        private async UniTaskVoid RunMatch()
        {
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
            EnablePlayerMovement();
            startDisplay.gameObject.SetActive(true);
            startDisplay.Display();
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

using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace StackBuild.UI.Scene.Title
{
    [RequireComponent(typeof(CanvasGroup))]
    public class TitleScreen : MonoBehaviour
    {

        [SerializeField] private CanvasGroup container;
        [SerializeField] private TMP_Text[] slideTexts;
        [SerializeField] private TMP_Text playText;
        [SerializeField] private TMP_Text copyrightText;

        private Tween playTextFlash;

        private void Reset()
        {
            container = GetComponent<CanvasGroup>();
        }

        private void Awake()
        {
            container.alpha = 0;
            container.interactable = false;

            playTextFlash = playText.DOFade(0, 1).From(1).SetEase(Ease.InQuad).SetLoops(-1, LoopType.Yoyo).Pause();
        }

        public async UniTask ShowAsync()
        {
            container.DOKill();
            container.alpha = 1;
            container.interactable = true;

            playTextFlash.Rewind();
            await ShowTexts(0.07f);
            _ = playTextFlash.Play();
        }

        public async UniTask HideAsync()
        {
            container.interactable = false;
            await container.DOFade(0, 0.2f);
        }

        private Sequence ShowTexts(float stagger)
        {
            var seq = DOTween.Sequence();

            int i = 0;
            foreach (var text in slideTexts)
            {
                seq = seq
                    .Join(text.DOFade(1, 0).From(0).SetDelay(i > 0 ? stagger : 0))
                    .Join(text.rectTransform.DOAnchorPosX(-200, TitleScene.SlideDecelerationDuration).From(true)
                        .SetEase(TitleScene.SlideDecelerationEasing));
                i++;
            }

            seq = seq.Join(copyrightText.DOFade(0, 1).From());

            return seq;
        }

    }
}

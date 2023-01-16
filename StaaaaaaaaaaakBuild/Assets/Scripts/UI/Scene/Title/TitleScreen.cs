using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace StackBuild.UI.Scene.Title
{
    public class TitleScreen : MonoBehaviour
    {

        [SerializeField] private RectTransform logoContainer;
        [SerializeField] private RectTransform iconContainer;
        [SerializeField] private Image[] iconParts;
        [SerializeField] private RectTransform titleMask;
        [SerializeField] private Image menuBackground;
        [SerializeField] private TMP_Text[] menuTexts;
        [FormerlySerializedAs("flashText")] [SerializeField] private TMP_Text playText;
        [SerializeField] private TMP_Text copyrightText;

        private Sequence sequence;

        private void Awake()
        {
            sequence = DOTween.Sequence()
                    .Append(ShowLogo())
                    .Append(ShowTexts(0.35f, Ease.InCubic, 0.07f, 0.75f, Ease.OutQuint))
                ;
            sequence.SetAutoKill(false);
            sequence.Pause();
        }

        private void Start()
        {
            DisplayAsync().Forget();
        }

        private async UniTaskVoid DisplayAsync()
        {
            sequence.Restart();
            await sequence.AsyncWaitForCompletion();
            FlashText();
        }

        private Sequence ShowLogo()
        {
            var seq = DOTween.Sequence();

            foreach (var part in iconParts)
            {
                seq = seq
                    .Join(part.DOFade(1, 0).From(0))
                    .Join(part.rectTransform.DOLocalMoveY(30, 0.15f).From().SetDelay(0.07f).SetEase(Ease.InQuad));
            }

            seq = seq
                .Append(iconContainer.DOAnchorPosX(0, 0.65f).From().SetEase(Ease.InOutQuart))
                .Join(DOTween
                    .To(() => titleMask.sizeDelta.x,
                        x => titleMask.sizeDelta = new Vector2(x, titleMask.sizeDelta.y), 0, 0.65f).From()
                    .SetEase(Ease.InOutQuart));

            return seq;
        }

        private Sequence ShowTexts(float accelerationDuration, Ease accelerationEase,
            float decelerationStagger, float decelerationDuration, Ease decelerationEase)
        {
            var seq = DOTween.Sequence();

            seq = seq
                .Append(logoContainer.DOAnchorMin(new Vector2(0.5f, 0.5f), accelerationDuration).From().SetEase(accelerationEase))
                .Join(logoContainer.DOAnchorMax(new Vector2(0.5f, 0.5f), accelerationDuration).From().SetEase(accelerationEase))
                .Join(logoContainer.DOPivotX(0.5f, accelerationDuration).From().SetEase(accelerationEase))
                .Join(logoContainer.DOAnchorPosX(0, accelerationDuration).From().SetEase(accelerationEase));

            seq = seq
                .Append(menuBackground.DOFade(0, decelerationDuration).From().SetEase(Ease.OutQuad))
                .Join(menuBackground.rectTransform.DOAnchorMin(new Vector2(0, 0), decelerationDuration).From().SetEase(decelerationEase));

            int i = 0;
            foreach (var text in menuTexts)
            {
                seq = seq
                    .Join(text.DOFade(1, 0).From(0).SetDelay(i > 0 ? decelerationStagger : 0).SetEase(decelerationEase))
                    .Join(text.rectTransform.DOAnchorPosX(-200, decelerationDuration).From(true)
                        .SetEase(decelerationEase));
                i++;
            }

            seq = seq.Join(copyrightText.DOFade(0, 1).From());

            return seq;
        }

        private void FlashText()
        {
            playText.DOFade(0, 1).From(1).SetEase(Ease.InQuad).SetLoops(-1, LoopType.Yoyo);
        }

    }
}

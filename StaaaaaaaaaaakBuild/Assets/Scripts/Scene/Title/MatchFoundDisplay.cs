using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StackBuild.Scene.Title
{
    public class MatchFoundDisplay : MonoBehaviour
    {
        [SerializeField] private CanvasGroup container;
        [SerializeField] private Image background;
        [SerializeField] private RectTransform[] horizontalLines;
        [SerializeField] private RectTransform[] verticalLines;
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text subtitleText;

        private void Awake()
        {
            container.alpha = 0;
        }

        public async UniTask DisplayAsync()
        {
            container.alpha = 1;

            var seq = DOTween.Sequence();

            foreach (var line in horizontalLines)
            {
                seq.Join(line.DOScaleX(0, 0.3f).From().SetEase(Ease.OutQuad));
            }

            foreach (var line in verticalLines)
            {
                seq.Join(line.DOScaleY(0, 0.3f).From().SetEase(Ease.OutQuad));
            }

            titleText.alpha = 0;
            seq
                .Append(background.material.DOFloat(1, "_Threshold", 0.333f).From(0).SetEase(Ease.OutQuad))
                .Join(DOVirtual.Float(1, 0, 0.2f, p => titleText.alpha = Mathf.Round(p)).SetDelay(0.15f)
                    .SetEase(Ease.Flash, 6))
                .Join(DOTween.To(() => titleText.characterSpacing, v => titleText.characterSpacing = v, 0, 1).From()
                    .SetEase(Ease.OutQuint))
                .Join(subtitleText.DOFade(1, 0.3f).From(0).SetDelay(0.2f));

            await seq.AsyncWaitForCompletion();
        }

    }
}
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace StackBuild.UI
{
    [RequireComponent(typeof(RectTransform))]
    public class FinishDisplay : MonoBehaviour
    {

        [SerializeField] private bool displayOnEnable;
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private CanvasGroup group;
        [SerializeField] private TMP_Text finishText;

        private void Reset()
        {
            rectTransform = GetComponent<RectTransform>();
            group = GetComponent<CanvasGroup>();
        }

        private void Awake()
        {
            group.alpha = 0;
        }

        private void OnEnable()
        {
            if (displayOnEnable)
            {
                Display();
            }
        }

        public void Display()
        {
            group.alpha = 1;
            DOTween.Sequence()
                .Append(rectTransform.DOScaleX(0, 0.15f).From().SetEase(Ease.OutQuad))
                .Append(rectTransform.DOSizeDelta(new Vector2(0, 8), 1f).From().SetEase(Ease.OutQuint))
                .Join(DOTween.To(() => finishText.characterSpacing, v => finishText.characterSpacing = v, 0, 1f).From()
                    .SetEase(Ease.OutQuint))
                .Join(group.DOFade(0, 0.3f).SetDelay(2));
        }

    }
}

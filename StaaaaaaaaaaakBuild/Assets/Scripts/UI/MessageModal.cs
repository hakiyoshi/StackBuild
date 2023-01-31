using Cysharp.Threading.Tasks;
using DG.Tweening;
using StackBuild.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StackBuild.UI
{
    public class MessageModal : MonoBehaviour
    {

        [SerializeField] private Image backdrop;
        [SerializeField] private RectTransform lineTop;
        [SerializeField] private RectTransform lineBottom;
        [SerializeField] private SkewedImage background;
        [SerializeField] private CanvasGroup content;
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text bodyText;
        [SerializeField] private RectTransform buttonContainer;

        public string Title
        {
            set
            {
                titleText.gameObject.SetActive(value != null);
                titleText.text = value;
            }
        }

        public string Body
        {
            set => bodyText.text = value;
        }

        public void AddButton(Transform button)
        {
            button.SetParent(buttonContainer, false);
        }

        public async UniTask ShowAsync()
        {
            await DOTween.Sequence()
                .Join(backdrop.DOFade(0, 0.1f).From())
                .Join(lineTop.DOScaleX(1, 0.2f).From(0).SetEase(Ease.OutQuad))
                .Join(lineBottom.DOScaleX(1, 0.2f).From(0).SetEase(Ease.OutQuad))
                .Join(background.rectTransform.DOAnchorMin(background.rectTransform.anchorMin.WithX(0.5f), 0.4f).From()
                    .SetDelay(0.15f).SetEase(Ease.OutQuart))
                .Join(background.rectTransform.DOAnchorMax(background.rectTransform.anchorMax.WithX(0.5f), 0.4f).From()
                    .SetEase(Ease.OutQuart))
                .Join(content.DOFade(1, 0.2f).From(0).SetDelay(0.05f));
            content.interactable = true;
        }

        public async UniTask HideAsync()
        {
            content.interactable = false;
            await DOTween.Sequence()
                .Join(content.DOFade(0, 0.15f))
                .Join(backdrop.DOFade(0, 0.15f))
                .Join(background.DOFade(0, 0.15f))
                .Join(lineTop.DOScaleX(0, 0.3f))
                .Join(lineBottom.DOScaleX(0, 0.3f));
        }

    }
}

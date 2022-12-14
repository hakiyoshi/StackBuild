using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace StackBuild.UI
{
    public class HUDSlide : MonoBehaviour
    {

        [SerializeField] private Vector2 amount;
        [SerializeField] private float duration;
        [SerializeField] private Ease easing;
        [SerializeField] private bool hideOnAwake;

        private Vector2 initialPosition;

        private void Awake()
        {
            var rectTransform = (RectTransform)transform;
            initialPosition = rectTransform.anchoredPosition;

            if (hideOnAwake)
            {
                rectTransform.anchoredPosition += amount;
            }
        }

        public async UniTaskVoid SlideInAsync()
        {
            gameObject.SetActive(true);
            await ((RectTransform)transform).DOAnchorPos(initialPosition, duration).SetEase(easing);
        }

        public async UniTaskVoid SlideOutAsync()
        {
            await ((RectTransform)transform).DOPivot(initialPosition + amount, duration).SetEase(easing);
            gameObject.SetActive(false);
        }

    }
}

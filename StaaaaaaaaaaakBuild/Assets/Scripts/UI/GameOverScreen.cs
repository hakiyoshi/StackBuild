using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace StackBuild.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class GameOverScreen : MonoBehaviour
    {

        [SerializeField] private bool hideOnAwake;
        [SerializeField] private CanvasGroup group;
        [SerializeField] private GameObject selectButtonOnShow;

        private void Reset()
        {
            group = GetComponent<CanvasGroup>();
        }

        private void Awake()
        {
            if(!hideOnAwake) return;
            group.alpha = 0;
            group.interactable = false;
        }

        public async UniTaskVoid ShowAsync()
        {
            await DOTween.Sequence()
                .Join(group.DOFade(1, 0.3f).From(0))
                .Join(((RectTransform)transform).DOSizeDelta(Vector2.zero, 0.5f).From().SetEase(Ease.OutQuart));
            group.interactable = true;
            EventSystem.current.SetSelectedGameObject(selectButtonOnShow);
        }

    }
}

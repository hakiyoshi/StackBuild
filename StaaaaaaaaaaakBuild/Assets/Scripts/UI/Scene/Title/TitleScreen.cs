using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

namespace StackBuild.UI.Scene.Title
{
    [RequireComponent(typeof(CanvasGroup))]
    public class TitleScreen : MonoBehaviour
    {

        [SerializeField] private InputActionReference inputStart;
        [SerializeField] private CanvasGroup container;
        [SerializeField] private TitleMenuStaggerDisplay staggerDisplay;
        [SerializeField] private TMP_Text playText;

        private readonly Subject<Unit> onStartPressed = new();

        public IObservable<Unit> OnStartPressed => onStartPressed;

        private void Reset()
        {
            container = GetComponent<CanvasGroup>();
        }

        private void Awake()
        {
            container.interactable = false;
            InputFromEvent.ActionMapFromEventPerformed(inputStart).Subscribe(_ =>
            {
                onStartPressed.OnNext(Unit.Default);
            }).AddTo(this);
        }

        public async UniTask ShowAsync()
        {
            container.DOKill();
            container.interactable = true;

            await staggerDisplay.Display();
            inputStart.action.Enable();
            _ = playText.DOFade(0, 1).From(1).SetEase(Ease.InQuad).SetLoops(-1, LoopType.Yoyo);
        }

        public void Hide()
        {
            inputStart.action.Disable();
            container.interactable = false;
            staggerDisplay.Hide();
            playText.DOKill();
        }

    }
}

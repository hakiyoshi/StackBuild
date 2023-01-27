using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using DG.Tweening.Core;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

namespace StackBuild.Scene.Title
{
    [RequireComponent(typeof(CanvasGroup))]
    public class TitleScreen : TitleSceneScreen
    {

        [SerializeField] private InputActionReference inputStart;
        [SerializeField] private CanvasGroup container;
        [SerializeField] private TitleMenuStaggerDisplay staggerDisplay;
        [SerializeField] private TMP_Text playText;

        private readonly Subject<Unit> onStartPressed = new();
        private Tween playTextFlash;

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
            playTextFlash = playText.DOFade(0, 1).From(1).SetEase(Ease.InQuad).SetLoops(-1, LoopType.Yoyo)
                .SetAutoKill(false).SetLink(gameObject).Pause();
        }

        public override async UniTask ShowAsync()
        {
            container.DOKill();
            container.interactable = true;
            container.alpha = 1;
            inputStart.action.Enable();

            await staggerDisplay.Display();
            playTextFlash.Restart();
        }

        public override async UniTask HideAsync()
        {
            inputStart.action.Disable();
            container.interactable = false;
            await container.DOFade(0, 0.07f);
            staggerDisplay.Hide();
            _ = playTextFlash.Pause();
        }

    }
}

using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace StackBuild.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class PracticeHUD : MonoBehaviour
    {

        [SerializeField] private InputActionReference inputExit;
        [SerializeField] private float exitHoldTime;
        [SerializeField] private CanvasGroup container;
        [SerializeField] private Slider exitBar;

        private bool isVisible;
        private bool isExitButtonDown;
        private bool exitConfirmed;

        private void Reset()
        {
            container = GetComponent<CanvasGroup>();
        }

        private void Awake()
        {
            InputFromEvent.ActionMapFromEventStarted(inputExit).Subscribe(_ =>
            {
                if (!isVisible || exitConfirmed || isExitButtonDown) return;
                isExitButtonDown = true;
                exitBar.DOValue(1, exitHoldTime).From(0).OnComplete(() => ExitAsync().Forget());
            }).AddTo(this);
            InputFromEvent.ActionMapFromEventCanceled(inputExit).Subscribe(_ =>
            {
                if (!isVisible || exitConfirmed) return;
                isExitButtonDown = false;
                exitBar.DOKill();
                exitBar.DOValue(0, 0.15f);
            }).AddTo(this);

            container.alpha = 0;
        }

        public async UniTaskVoid ShowAsync()
        {
            isVisible = true;
            inputExit.action.Enable();
            await container.DOFade(1, 0.5f);
        }

        private async UniTaskVoid ExitAsync()
        {
            exitConfirmed = true;
            inputExit.action.Disable();
            await LoadingScreen.Instance.ShowAsync();
            SceneManager.LoadSceneAsync("MainMenu");
        }

    }
}
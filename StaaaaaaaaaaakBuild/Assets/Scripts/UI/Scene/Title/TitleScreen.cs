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
        [SerializeField] private TitleMenuStaggerDisplay staggerDisplay;
        [SerializeField] private TMP_Text playText;

        private void Reset()
        {
            container = GetComponent<CanvasGroup>();
        }

        private void Awake()
        {
            container.interactable = false;
        }

        public async UniTask ShowAsync()
        {
            container.DOKill();
            container.interactable = true;

            await staggerDisplay.Display();
            _ = playText.DOFade(0, 1).From(1).SetEase(Ease.InQuad).SetLoops(-1, LoopType.Yoyo);
        }

        public void Hide()
        {
            container.interactable = false;
            staggerDisplay.Hide();
            playText.DOKill();
        }

    }
}

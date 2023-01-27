using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace StackBuild.UI
{
    public class LoadingScreen : MonoBehaviour
    {

        public static readonly Color DefaultColor = new(0.03137255f, 0.18039216f, 0.27450982f, a: 1);

        private static Color backgroundColor;

        public static LoadingScreen Instance { get; private set; }

        [SerializeField] private Image fadeBackground;
        [SerializeField] private Image trianglesBackground;
        [SerializeField] private CanvasGroup contents;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(Instance.gameObject);
            }
            Instance = this;

            fadeBackground.color = backgroundColor;
            trianglesBackground.color = backgroundColor;

            contents.alpha = 1;
            fadeBackground.enabled = true;
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        public async UniTask ShowAsync(LoadingScreenType type = LoadingScreenType.Fade, Color? color = null)
        {
            backgroundColor = color ?? DefaultColor;

            fadeBackground.enabled = type == LoadingScreenType.Fade;
            trianglesBackground.enabled = type == LoadingScreenType.Triangles;

            var seq = DOTween.Sequence()
                .Join(contents.DOFade(1, 0.4f).From(0).SetEase(Ease.Linear));

            switch (type)
            {
                case LoadingScreenType.Fade:
                    fadeBackground.color = backgroundColor;
                    break;
                case LoadingScreenType.Triangles:
                    trianglesBackground.color = backgroundColor;
                    seq.Join(trianglesBackground.material.DOFloat(1, "_Threshold", 0.333f).From(0).SetEase(Ease.OutQuad));
                    break;
                default: throw new NotImplementedException(type.ToString());
            }

            await seq.AsyncWaitForCompletion();
        }

        // ShowAsync(...)で表示したものを隠す
        // 表示したときと違うtypeが使えて楽しい！
        // 背景色は表示したときのものがそのまま使われる
        public async UniTask HideAsync(LoadingScreenType type = LoadingScreenType.Fade)
        {
            fadeBackground.enabled = type == LoadingScreenType.Fade;
            trianglesBackground.enabled = type == LoadingScreenType.Triangles;

            var seq = DOTween.Sequence()
                .Join(contents.DOFade(0, 0.25f).From(1).SetEase(Ease.Linear));

            switch (type)
            {
                case LoadingScreenType.Fade:
                    fadeBackground.color = backgroundColor;
                    break;
                case LoadingScreenType.Triangles:
                    trianglesBackground.color = backgroundColor;
                    seq.Join(trianglesBackground.material.DOFloat(0, "_Threshold", 0.333f).From(1).SetEase(Ease.OutQuad));
                    break;
                default: throw new NotImplementedException(type.ToString());
            }

            await seq.AsyncWaitForCompletion();
        }

    }
}

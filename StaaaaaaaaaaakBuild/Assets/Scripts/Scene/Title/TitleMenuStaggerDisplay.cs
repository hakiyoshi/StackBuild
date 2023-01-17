using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace StackBuild.UI.Scene.Title
{
    public class TitleMenuStaggerDisplay : MonoBehaviour
    {

        private const float DefaultStagger = 0.07f;

        [SerializeField] private bool hideOnAwake;
        [SerializeField] private Graphic[] items;

        private void Awake()
        {
            if (hideOnAwake)
            {
                Hide();
            }
        }

        public Sequence Display()
        {

            var seq = DOTween.Sequence();

            int i = 0;
            foreach (var item in items)
            {
                seq
                    .Join(item.DOFade(1, 0).From(0).SetDelay(i > 0 ? DefaultStagger : 0))
                    .Join(((RectTransform)item.transform).DOAnchorPosX(-200, TitleScene.SlideDecelerationDuration).From(true)
                        .SetEase(TitleScene.SlideDecelerationEasing));
                i++;
            }

            return seq;
        }

        public void Hide()
        {
            foreach (var item in items)
            {
                item.DOKill();
                var color = item.color;
                color.a = 0;
                item.color = color;
            }
        }

    }
}

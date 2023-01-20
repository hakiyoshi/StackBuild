using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace StackBuild.Scene.Title
{
    public class TitleMenuStaggerDisplay : MonoBehaviour
    {

        private const float DefaultStagger = 0.07f;

        [SerializeField] private bool hideOnAwake;
        [SerializeField] private Graphic[] items;
        private Sequence seq;

        private void Awake()
        {
            if (hideOnAwake)
            {
                Hide();
            }
        }

        public Sequence Display()
        {
            seq = DOTween.Sequence();

            int i = 0;
            foreach (var item in items)
            {
                seq
                    .Join(item.DOFade(1, 0).From(0).SetDelay(i > 0 ? DefaultStagger : 0))
                    .Join(((RectTransform)item.transform).DOAnchorPosX(0, TitleScene.SlideDecelerationDuration).From(new Vector2(-200, 0))
                        .SetEase(TitleScene.SlideDecelerationEasing));
                i++;
            }

            return seq;
        }

        public void Hide()
        {
            seq?.Kill();
            foreach (var item in items)
            {
                var color = item.color;
                color.a = 0;
                item.color = color;
            }
        }

    }
}

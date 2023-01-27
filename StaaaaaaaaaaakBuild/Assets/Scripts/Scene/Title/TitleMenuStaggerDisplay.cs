using System;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace StackBuild.Scene.Title
{
    public class TitleMenuStaggerDisplay : MonoBehaviour
    {

        [Serializable]
        private struct ItemComponentInfo
        {
            public Graphic graphic;
            public CanvasGroup group;
        }

        private const float DefaultStagger = 0.07f;

        [SerializeField] private bool hideOnAwake;
        [SerializeField] private RectTransform[] items;
        private Sequence seq;
        private ItemComponentInfo[] itemComponents;

        private void Awake()
        {
            itemComponents = items.Select(item => new ItemComponentInfo
            {
                graphic = item.GetComponent<Graphic>(),
                group = item.GetComponent<CanvasGroup>(),
            }).ToArray();

            if (hideOnAwake)
            {
                Hide();
            }
        }

        public Sequence Display()
        {
            seq = DOTween.Sequence();

            for (int i = 0; i < items.Length; i++)
            {
                var item = items[i];
                var comp = itemComponents[i];

                if (comp.graphic != null)
                {
                    seq.Insert(i * DefaultStagger, comp.graphic.DOFade(1, 0).From(0));
                }
                else if (comp.group != null)
                {
                    seq.Insert(i * DefaultStagger, comp.group.DOFade(1, 0).From(0));
                }
                else
                {
                    seq.InsertCallback(i * DefaultStagger, () => item.gameObject.SetActive(true));
                }

                seq.Join(item.DOAnchorPosX(0, TitleScene.SlideDecelerationDuration).From(new Vector2(-200, 0))
                    .SetEase(TitleScene.SlideDecelerationEasing));
            }

            return seq;
        }

        public void Hide()
        {
            seq?.Kill();
            for (int i = 0; i < items.Length; i++)
            {
                var item = items[i];
                var comp = itemComponents[i];

                if (comp.graphic != null)
                {
                    var color = comp.graphic.color;
                    color.a = 0;
                    comp.graphic.color = color;
                }
                else if (comp.group != null)
                {
                    comp.group.alpha = 0;
                }
                else
                {
                    item.gameObject.SetActive(false);
                }
            }
        }

    }
}

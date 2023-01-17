using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace StackBuild.UI.Scene.Title
{
    public class TitleLogo : MonoBehaviour
    {

        [SerializeField] private RectTransform iconContainer;
        [SerializeField] private Image[] iconParts;
        [SerializeField] private RectTransform titleMask;

        private Sequence sequence;

        private void Awake()
        {
            sequence = DOTween.Sequence()
                    .Append(ShowLogo())
                ;
            sequence.SetAutoKill(false);
            sequence.Pause();
        }

        public async UniTask DisplayAsync()
        {
            sequence.Restart();
            await sequence.AsyncWaitForCompletion();
        }

        private Sequence ShowLogo()
        {
            var seq = DOTween.Sequence();

            foreach (var part in iconParts)
            {
                seq = seq
                    .Join(part.DOFade(1, 0).From(0))
                    .Join(part.rectTransform.DOLocalMoveY(30, 0.15f).From().SetDelay(0.07f).SetEase(Ease.InQuad));
            }

            seq = seq
                .Append(iconContainer.DOAnchorPosX(0, 0.65f).From().SetEase(Ease.InOutQuart))
                .Join(DOTween
                    .To(() => titleMask.sizeDelta.x,
                        x => titleMask.sizeDelta = new Vector2(x, titleMask.sizeDelta.y), 0, 0.65f).From()
                    .SetEase(Ease.InOutQuart));

            return seq;
        }

    }
}

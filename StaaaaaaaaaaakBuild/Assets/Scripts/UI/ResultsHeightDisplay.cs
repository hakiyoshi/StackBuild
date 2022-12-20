using System.Collections;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StackBuild.UI
{
    public class ResultsHeightDisplay : MonoBehaviour
    {

        [SerializeField] private CanvasGroup group;
        [SerializeField] private TMP_Text heightText;
        [SerializeField] private TMP_Text winLoseText;
        [SerializeField] private Image loseBackground;

        private void Awake()
        {
            group.alpha = 0;
        }

        public void SetVisible(bool visible, bool fade)
        {
            group.DOFade(visible ? 1 : 0, fade ? 0.2f : 0);
        }

        public void DisplayHeight(float height)
        {
            heightText.text = $"<mspace=0.6em>{height:F0}</mspace>m";
        }

        public async UniTaskVoid DisplayWinAsync()
        {
            winLoseText.text = "WIN";
            await DisplayWinLoseCommon()
                .Join(winLoseText.rectTransform.DOScale(1, 0.5f).From(3).SetEase(Ease.OutQuart))
                .Join(winLoseText.DOColor(new Color(0.96f, 0.56f, 0.05f), 0.2f).SetEase((time, duration, _, _) =>
                    time < duration && time % 0.1f >= 0.05f ? 0 : 1))
                .Join(loseBackground.DOFade(1, 0.3f).SetDelay(0.125f));
        }

        public async UniTaskVoid DisplayLoseAsync()
        {
            winLoseText.text = "LOSE";
            await DisplayWinLoseCommon()
                .Join(loseBackground.DOFade(1, 0.3f).SetDelay(0.125f));
        }

        private Sequence DisplayWinLoseCommon()
        {
            return DOTween.Sequence()
                .Join(heightText.rectTransform.DOScale(0.3f, 0.25f).SetEase(Ease.InCubic))
                .Append(winLoseText.DOFade(0, 0).From())
                .Append(heightText.rectTransform.DOLocalMoveY(110, 0.375f).From(0).SetEase(Ease.OutCubic))
                .Join(DOTween.To(() => winLoseText.characterSpacing, v => winLoseText.characterSpacing = v, 50, 0.5f).From().SetEase(Ease.OutQuart));
        }

    }
}

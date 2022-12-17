using DG.Tweening;
using TMPro;
using UnityEngine;

namespace StackBuild.UI
{
    public class StartDisplay : MonoBehaviour
    {

        [SerializeField] private bool displayOnEnable;
        [SerializeField] private float scale;
        [SerializeField] private TMP_Text startText;

        private void OnEnable()
        {
            if (displayOnEnable)
            {
                Display();
            }
        }

        public void Display()
        {
            DOTween.Sequence()
                .Join(startText.rectTransform.DOScale(scale, 1f).From().SetEase(Ease.OutQuart))
                .Join(startText.DOFade(0, 0.2f).From(1).SetDelay(0.8f));
        }

    }
}

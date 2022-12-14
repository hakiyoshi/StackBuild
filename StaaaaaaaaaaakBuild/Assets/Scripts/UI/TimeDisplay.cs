using DG.Tweening;
using TMPro;
using UnityEngine;

namespace StackBuild.UI
{
    public class TimeDisplay : MonoBehaviour
    {

        [SerializeField] private TMP_Text text;
        [SerializeField] private Color flashColor;

        public void Display(int seconds, bool flash = false)
        {
            text.text = $"{seconds / 60}:{seconds % 60:D2}";
            if (!flash) return;
            text.DOComplete();
            text.DOColor(text.color, 0.3f).From(flashColor);
        }

    }
}

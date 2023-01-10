using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

namespace StackBuild.UI
{
    public class HeightMeter : MonoBehaviour
    {

        [SerializeField] private float value;
        [SerializeField] private float scale;
        [SerializeField] private float interval;
        [SerializeField] private bool alignRight;
        [SerializeField] private RectTransform bar;
        [SerializeField] private Image imageLines;
        [SerializeField] private TextMeshProUGUI labelPrefab;
        [SerializeField] private RectTransform labelContainer;
        private TextMeshProUGUI[] labels;

        private float tweenedValue;

        private void Start()
        {
            float height = ((RectTransform)transform).rect.height;
            float linesTextureHeight = imageLines.sprite.texture.height;
            int linesTextureRepeat = 1 + Mathf.FloorToInt(height / linesTextureHeight) | 1; // x / 2 * 2 + 1 == x | 1
            var linesSize = imageLines.rectTransform.sizeDelta;
            linesSize.y = linesTextureRepeat * linesTextureHeight;
            imageLines.rectTransform.sizeDelta = linesSize;

            labels = new TextMeshProUGUI[Mathf.CeilToInt(height / scale / interval) + 1];
            for (int i = 0; i < labels.Length; i++)
            {
                labels[i] = Instantiate(labelPrefab, labelContainer != null ? labelContainer : transform);
                labels[i].rectTransform.pivot = new Vector2(alignRight ? 1 : 0, 0);
                labels[i].rectTransform.anchorMin = new Vector2(alignRight ? 1 : 0, 0);
                labels[i].rectTransform.anchorMax = new Vector2(alignRight ? 1 : 0, 0);
            }

            tweenedValue = value;
            Display(value);
        }

        public void Add(float amount)
        {
            Set(value + amount);
        }

        public void Set(float value)
        {
            this.value = value;

            DOTween.To(() => tweenedValue, v =>
            {
                tweenedValue = v;
                Display(v);
            }, value, 0.5f).SetTarget(this).SetEase(Ease.OutCubic);
        }

        private void Display(float v)
        {
            float height = ((RectTransform)transform).rect.height;
            int middleRow = Mathf.FloorToInt(height / 2 / scale);

            // バー
            var barSize = bar.sizeDelta;
            barSize.y = Mathf.Max(2, Mathf.Min(middleRow, v) * scale);
            bar.sizeDelta = barSize;

            // グリッド
            float scroll = Mathf.Max(0, v - middleRow);
            var gridPos = imageLines.rectTransform.anchoredPosition;
            gridPos.y = -(scroll * scale % 50);
            imageLines.rectTransform.anchoredPosition = gridPos;

            // ラベル
            int startRow = Mathf.RoundToInt(scroll / interval) - Mathf.FloorToInt(height / 2 / scale / interval);
            for (int i = 0; i < labels.Length; i++)
            {
                labels[i].text = ((i + startRow) * interval).ToString();
                var labelPos = labels[i].rectTransform.anchoredPosition;
                labelPos.y = (-scroll + (i + startRow) * interval) * scale;
                labels[i].rectTransform.anchoredPosition = labelPos;
            }
        }

    }
}

using System.Linq;
using Cinemachine;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StackBuild.UI
{
    public class IntroDisplay : MonoBehaviour
    {

        [SerializeField] private bool displayOnEnable;
        [SerializeField] private CinemachineVirtualCamera vcam;
        [SerializeField] private CanvasGroup group;
        [SerializeField] private RectTransform iconContainer;
        [SerializeField] private Image[] iconParts;
        [SerializeField] private RectTransform logoTextMask;
        [SerializeField] private RectTransform titleLineTop;
        [SerializeField] private RectTransform titleLineBottom;
        [SerializeField] private RectTransform titleBackground;
        [SerializeField] private Image[] titleBackgroundCorners;
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private Image accentLineTop;
        [SerializeField] private Image accentLineBottom;
        [SerializeField] private RectTransform frameTop;
        [SerializeField] private RectTransform frameBottom;
        [SerializeField] private RectTransform[] frameLines;
        [SerializeField] private RectTransform[] mapInfoRows;

        private Sequence sequence;
        private CinemachineTrackedDolly dolly;

        private void Awake()
        {
            dolly = vcam.GetCinemachineComponent<CinemachineTrackedDolly>();
            // Display()でSequence作ってSetActive(true)と一緒に呼ぶと600msくらいかかる あほか
            // なので先に作って停止したSequenceを持っておく
            // ついでにCanvas activeのままにできるようになった (active化でレイアウト走るのでその分の軽量化)
            // 11msまで軽くなったのでもうこれでいいかな
            sequence = DOTween.Sequence()
                // ★タイミング調整はこちら★
                .Insert(0,
                    DOTween.To(() => dolly.m_PathPosition, v => dolly.m_PathPosition = v, 1, 3).From(0)
                        .SetEase(Ease.OutQuint))
                .Insert(0, ShowLogo())
                .Insert(1.2f + 0, ShowTitleLine(titleLineTop, true, 0.5f, Ease.OutQuart))
                .Insert(1.2f + 0, ShowTitleLine(titleLineBottom, false, 0.5f, Ease.OutQuart))
                .Insert(1.2f + 0.38f, OpenTitleBackground(0.12f).Append(ShowTitleText()))
                .Insert(1.2f + 0.25f, ShowAccentLine(accentLineTop, true, 0.65f, Ease.OutQuart))
                .Insert(1.2f + 0.25f, ShowAccentLine(accentLineBottom, false, 0.65f, Ease.OutQuart))
                .Insert(1.2f + 0, ShowFrameLines(0.5f, Ease.OutQuart))
                .Insert(1.2f + 0.75f, MoveFrames(20, 0.4f, Ease.OutQuart))
                .Insert(1.2f + 0, ShowMapInfo(300, 0.05f, 0.5f, Ease.OutQuart))
                .SetAutoKill(false).SetLink(gameObject).Pause();
        }

        private void OnEnable()
        {
            if (displayOnEnable)
            {
                Display();
            }
        }

        public void Display()
        {
            vcam.enabled = true;
            group.alpha = 1;
            sequence.Restart();
        }

        public void DisableVirtualCamera()
        {
            vcam.enabled = false;
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
                .Append(iconContainer.DOLocalMoveX(0, 0.65f).From().SetEase(Ease.InOutQuart))
                .Join(DOTween
                    .To(() => logoTextMask.sizeDelta.x,
                        x => logoTextMask.sizeDelta = new Vector2(x, logoTextMask.sizeDelta.y), 0, 0.65f).From()
                    .SetEase(Ease.InOutQuart))
                .Append(iconContainer.DOLocalMoveY(0, 0.3f).From().SetEase(Ease.InOutCubic))
                .Join(logoTextMask.DOLocalMoveY(0, 0.3f).From().SetEase(Ease.InOutCubic));

            return seq;
        }

        private Sequence ShowTitleLine(RectTransform line, bool fromRight, float duration, Ease ease)
        {
            line.anchorMin = line.anchorMax = new Vector2(fromRight ? 1 : 0, 0.5f);
            line.pivot = new Vector2(fromRight ? 0 : 1, 0);
            return DOTween.Sequence()
                .Join(line.DOAnchorMin(new Vector2(0.5f, 0.5f), duration).SetEase(ease))
                .Join(line.DOAnchorMax(new Vector2(0.5f, 0.5f), duration).SetEase(ease))
                .Join(line.DOPivotX(0.5f, duration).SetEase(ease));
        }

        private Sequence OpenTitleBackground(float duration)
        {
            var seq = DOTween.Sequence()
                .Join(titleBackground.DOScaleY(1, duration).From(0));

            foreach (var corner in titleBackgroundCorners)
            {
                seq = seq.Join(corner.DOFade(1, 0).From(0))
                    .Join(corner.rectTransform.DOLocalMoveY(corner.rectTransform.localPosition.y, duration).From(0));
            }

            return seq;
        }

        private Sequence ShowTitleText()
        {
            var originalText = titleText.text;
            return DOTween.Sequence()
                .Join(titleText.DOFade(1, 0).From(0))
                .Join(DOVirtual.Int(titleText.text.Length, 0, 0.35f,
                    v => titleText.text = ShuffleLastChars(originalText, v)));
        }

        // 文字列の最後のn文字をシャッフル
        private static string ShuffleLastChars(string str, int n)
        {
            if (n <= 1) return str;

            var chars = str.ToCharArray();
            for (int i = Mathf.Max(0, chars.Length - n); i < chars.Length - 2; i++)
            {
                int k = Random.Range(i + 1, chars.Length);
                (chars[i], chars[k]) = (chars[k], chars[i]);
            }

            return new string(chars);
        }

        private Sequence ShowAccentLine(Image line, bool fromRight, float duration, Ease ease)
        {
            var rt = line.rectTransform;
            rt.anchorMin = rt.anchorMax = new Vector2(fromRight ? 1 : 0, 0.5f);
            rt.pivot = new Vector2(fromRight ? 0 : 1, 0);
            return DOTween.Sequence()
                .Join(rt.DOAnchorMin(new Vector2(0.5f, 0.5f), duration).SetEase(ease))
                .Join(rt.DOAnchorMax(new Vector2(0.5f, 0.5f), duration).SetEase(ease))
                .Join(rt.DOScaleX(1, duration).From(0).SetEase(ease))
                .Join(rt.DOPivotX(0.5f, duration).SetEase(ease))
                .Join(line.DOFade(0, duration).From(1).SetEase(Ease.InQuart));
        }

        private Sequence ShowFrameLines(float duration, Ease ease)
        {
            var seq = DOTween.Sequence();

            foreach (var line in frameLines)
            {
                seq = seq
                    .Join(line.DOAnchorPosX(0, duration).From().SetEase(ease))
                    .Join(line.DOScaleX(1, duration + 0.25f).From(2).SetEase(ease));
            }

            return seq;
        }

        private Sequence MoveFrames(float fromY, float duration, Ease ease)
        {
            return DOTween.Sequence()
                .Join(frameTop.DOAnchorPosY(-fromY, duration).From().SetEase(ease))
                .Join(frameBottom.DOAnchorPosY(fromY, duration).From().SetEase(ease));
        }

        private Sequence ShowMapInfo(float relX, float stagger, float duration, Ease ease)
        {
            var seq = DOTween.Sequence();

            int i = 0;
            foreach (var row in mapInfoRows.Reverse())
            {
                seq = seq
                    .Join(row.DOAnchorPosX(relX, duration).From(true).SetDelay(i > 0 ? stagger : 0).SetEase(ease));
                i++;
            }

            return seq;
        }

    }
}

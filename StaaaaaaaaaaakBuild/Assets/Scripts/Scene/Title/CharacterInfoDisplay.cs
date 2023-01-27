using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using StackBuild.Extensions;

namespace StackBuild.Scene.Title
{
    public class CharacterInfoDisplay : MonoBehaviour
    {

        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private LayoutGroup layoutGroup;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text dashDescriptionText;
        [SerializeField] private Slider speedMeter;
        [SerializeField] private Slider powerMeter;

        private void Reset()
        {
            rectTransform = GetComponent<RectTransform>();
            canvasGroup = GetComponent<CanvasGroup>();
            layoutGroup = GetComponent<LayoutGroup>();
        }

        public async UniTaskVoid DisplayAsync(CharacterProperty character)
        {
            canvasGroup.alpha = character == null ? 0 : 1;
            if (character == null)
            {
                rectTransform.DOKill();
                rectTransform.sizeDelta = rectTransform.sizeDelta.WithY(80);
                return;
            }

            nameText.text = character.Name;
            dashDescriptionText.text = character.DashDescription;

            // 高さをアニメーションするのでレイアウトを再計算
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
            var size = new Vector2(rectTransform.sizeDelta.x, layoutGroup.preferredHeight);

            await DOTween.Sequence()
                .Join(speedMeter.DOValue(character.Move.MaxSpeed, TitleScene.SlideDecelerationDuration)
                    .From(0).SetEase(TitleScene.SlideDecelerationEasing))
                .Join(powerMeter.DOValue(character.Attack.KnockbackPower, TitleScene.SlideDecelerationDuration)
                    .From(0).SetEase(TitleScene.SlideDecelerationEasing))
                .Join(rectTransform.DOSizeDelta(size, TitleScene.SlideDecelerationDuration)
                    .SetEase(TitleScene.SlideDecelerationEasing));
        }

    }
}
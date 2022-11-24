using System;
using DG.Tweening;
using UniRx;
using UnityEngine;

namespace StackBuild
{
    public class CatchAnimation : MonoBehaviour
    {
        [SerializeField] private InputSender inputSender;
        [SerializeField] private PlayerProperty playerProperty;
        [SerializeField] private Transform CatchEffectObject;

        private Vector3 startScale;


        private void Start()
        {
            startScale = CatchEffectObject.localScale;
            CatchEffectObject.gameObject.SetActive(false);

            inputSender.Catch.Subscribe(x =>
            {
                if (x)
                {
                    CatchEffectObject.gameObject.SetActive(true);
                    CatchEffectObject
                        .DOScale(CatchEffectObject.localScale + playerProperty.characterProperty.CatchEffectMaxSizeOffset,
                            playerProperty.characterProperty.CatchEffectAppearanceTime);
                }
                else
                {
                    CatchEffectObject.DOScale(startScale, playerProperty.characterProperty.CatchEffectDisappearingTime)
                        .OnComplete(() =>
                        {
                            CatchEffectObject.gameObject.SetActive(false);
                        }).Play();
                }
            }).AddTo(this);
        }
    }
}
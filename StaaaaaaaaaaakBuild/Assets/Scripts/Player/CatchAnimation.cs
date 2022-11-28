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

        private CharacterProperty property
        {
            get
            {
                return playerProperty.characterProperty;
            }
        }

        private Vector3 startScale;


        private void Start()
        {
            //初期化
            startScale = CatchEffectObject.localScale;
            CatchEffectObject.gameObject.SetActive(false);

            inputSender.Catch.Subscribe(x =>
            {
                if (x)
                {
                    //掴んだ場合
                    CatchEffectObject.gameObject.SetActive(true);
                    CatchEffectObject
                        .DOScale(CatchEffectObject.localScale + property.Catch.CatchEffectMaxSizeOffset,
                            property.Catch.CatchEffectAppearanceTime);
                }
                else
                {
                    //離した場合
                    CatchEffectObject.DOScale(startScale, property.Catch.CatchEffectDisappearingTime)
                        .OnComplete(() =>
                        {
                            CatchEffectObject.gameObject.SetActive(false);
                        });
                }
            }).AddTo(this);
        }
    }
}
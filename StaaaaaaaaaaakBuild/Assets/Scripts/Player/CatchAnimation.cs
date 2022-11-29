using System;
using DG.Tweening;
using UniRx;
using Unity.Netcode;
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

            inputSender.Catch.Subscribe(x =>
            {
                if (x)
                {
                    //掴んだ場合
                    CatchEffectObject
                        .DOScale(CatchEffectObject.localScale + property.Catch.CatchEffectMaxSizeOffset,
                            property.Catch.CatchEffectAppearanceTime);
                }
                else
                {
                    //離した場合
                    CatchEffectObject.DOScale(startScale, property.Catch.CatchEffectDisappearingTime);
                }
            }).AddTo(this);
        }
    }
}
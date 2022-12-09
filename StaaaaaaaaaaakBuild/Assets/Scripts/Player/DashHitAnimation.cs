using System;
using DG.Tweening;
using UniRx;
using UnityEngine;

namespace StackBuild
{
    public class DashHitAnimation : MonoBehaviour
    {
        [SerializeField] private PlayerProperty playerProperty;

        private void Start()
        {
            playerProperty.HitDashAttack.Subscribe(x =>
            {
                if (x.playerProperty.characterProperty.Attack.StunTime != 0.0f)
                {
                    transform.DOShakeRotation(x.playerProperty.characterProperty.Attack.StunTime, 10.0f, 5, 10f);
                }
                else
                {
                    transform.DOShakeRotation(1.0f, 1.0f, 5, 10f);
                }

            }).AddTo(this);
        }
    }
}
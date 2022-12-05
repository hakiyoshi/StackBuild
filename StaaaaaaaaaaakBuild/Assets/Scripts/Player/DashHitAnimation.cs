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
            playerProperty.DashHitAction.Subscribe(x =>
            {
                transform.DOShakeRotation(x.characterProperty.Dash.Attack.StunTime, 10.0f, 5, 10f);

            }).AddTo(this);
        }
    }
}
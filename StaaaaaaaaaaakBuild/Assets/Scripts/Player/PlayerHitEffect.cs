using System;
using UniRx;
using UnityEngine;

namespace StackBuild
{
    public class PlayerHitEffect : MonoBehaviour
    {
        [SerializeField] private PlayerProperty playerProperty;

        private void Start()
        {
            playerProperty.HitDashAttack.Subscribe(x =>
            {
                
            }).AddTo(this);
        }
    }
}
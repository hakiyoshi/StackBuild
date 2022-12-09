using System;
using StackBuild.Particle;
using UniRx;
using UnityEngine;

namespace StackBuild
{
    public class PlayerHitEffect : MonoBehaviour
    {
        [SerializeField] private PlayerProperty playerProperty;

        [SerializeField] private ParticleSetup particle;

        private void Start()
        {
            playerProperty.HitDashAttack.Subscribe(x =>
            {

            }).AddTo(this);
        }
    }
}
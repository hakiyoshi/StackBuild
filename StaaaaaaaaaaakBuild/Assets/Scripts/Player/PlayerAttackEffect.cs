using System;
using StackBuild.Particle;
using UniRx;
using UnityEngine;

namespace StackBuild
{
    public class PlayerAttackEffect : MonoBehaviour
    {
        [SerializeField] private PlayerProperty playerProperty;

        [SerializeField] private ParticleSetup particle;

        private void Start()
        {
            playerProperty.HitDashAttack.Subscribe(x =>
            {
                if (x.playerProperty.characterProperty.Attack.StunTime != 0)
                    StunParticle(x.HitPoint);

                HitParticle(x.HitPoint);
            }).AddTo(this);
        }

        void StunParticle(in Vector3 hitPoint)
        {
            //particle.Stun.transform.position = hitPoint;
            particle.Stun.Play();
        }

        void HitParticle(in Vector3 hitPoint)
        {
            particle.Hit.transform.position = hitPoint;
            particle.Hit.Play();
        }
    }
}
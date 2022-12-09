using System;
using Unity.VisualScripting;
using UnityEngine;

namespace StackBuild.Particle
{
    public class ParticleSetup : MonoBehaviour
    {
        [SerializeField] private PlayerProperty playerProperty;
        [field: SerializeField] public ParticleSystem Stun { get; private set; }
        public ParticleSystem Hit { get; private set; }

        private void Start()
        {
            //スタン
            //StunSetup();

            //ヒット
            HitSetup();
        }

        void StunSetup()
        {
            var main = Stun.main;
            main.duration = playerProperty.characterProperty.Attack.StunTime;
            foreach (Transform child in Stun.transform)
            {
                if (!child.TryGetComponent(out ParticleSystem particle))
                    continue;

                main = particle.main;
                main.duration = playerProperty.characterProperty.Attack.StunTime;
            }
        }

        void HitSetup()
        {
            Hit = Instantiate(playerProperty.characterProperty.Model.HitEffect, this.transform)
                .GetComponent<ParticleSystem>();
        }
    }
}
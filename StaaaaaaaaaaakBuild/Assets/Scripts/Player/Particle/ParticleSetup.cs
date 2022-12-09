using System;
using UnityEngine;

namespace StackBuild.Particle
{
    public class ParticleSetup : MonoBehaviour
    {
        [SerializeField] private PlayerProperty playerProperty;
        [SerializeField] private ParticleSystem stunParticle;

        private void Start()
        {
            //スタン
            StunSetup();
        }

        void StunSetup()
        {
            var main = stunParticle.main;
            main.duration = playerProperty.characterProperty.Attack.StunTime;
            foreach (Transform child in stunParticle.transform)
            {
                if (!child.TryGetComponent(out ParticleSystem particle))
                    continue;

                main = particle.main;
                main.duration = playerProperty.characterProperty.Attack.StunTime;
            }
        }
    }
}
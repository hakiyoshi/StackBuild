using System;
using UnityEngine;

namespace StackBuild.Particle
{
    public class ParticleSetup : MonoBehaviour
    {
        [SerializeField] private PlayerProperty playerProperty;
        [field: SerializeField] public ParticleSystem StunParticle { get; private set; }

        private void Start()
        {
            //スタン
            StunSetup();
        }

        void StunSetup()
        {
            var main = StunParticle.main;
            main.duration = playerProperty.characterProperty.Attack.StunTime;
            foreach (Transform child in StunParticle.transform)
            {
                if (!child.TryGetComponent(out ParticleSystem particle))
                    continue;

                main = particle.main;
                main.duration = playerProperty.characterProperty.Attack.StunTime;
            }
        }
    }
}
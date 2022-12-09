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

        }

        void StunSetup()
        {
            var main = stunParticle.main;
        }
    }
}
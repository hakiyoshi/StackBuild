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

        private Vector3 hitPosition;

        private void Start()
        {
            playerProperty.HitDashAttack.Subscribe(x =>
            {
                particle.StunParticle.transform.position = hitPosition;
                particle.StunParticle.Play();
            }).AddTo(this);
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            hitPosition = hit.point;
        }
    }
}
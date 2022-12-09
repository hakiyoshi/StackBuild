using System;
using DG.Tweening;
using UniRx;
using UnityEngine;

namespace StackBuild
{
    public class PlayerKnockback : MonoBehaviour
    {
        [SerializeField] private PlayerProperty playerProperty;

        private CharacterController characterController;
        private Vector3 velocity = Vector3.zero;

        private void Start()
        {
            TryGetComponent(out characterController);

            playerProperty.HitDashAttack.Subscribe(x =>
            {

                var dir = (transform.position - x.playerProperty.PlayerObject.transform.position).normalized;
                var vec = dir * x.playerProperty.characterProperty.Attack.KnockbackPower;

                DOVirtual.Vector3(vec, Vector3.zero, x.playerProperty.characterProperty.Attack.KnockbackTime,
                    value => velocity = value).SetEase(x.playerProperty.characterProperty.Attack.KnockbackEase);
            }).AddTo(this);
        }

        private void Update()
        {
            characterController.Move(velocity * Time.deltaTime);
        }
    }
}
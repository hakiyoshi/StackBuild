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

            playerProperty.DashHitAction.Subscribe(x =>
            {

                var dir = (transform.position - x.PlayerObject.transform.position).normalized;
                var vec = dir * x.characterProperty.Dash.Attack.KnockbackPower;

                DOVirtual.Vector3(vec, Vector3.zero, x.characterProperty.Dash.Attack.KnockbackTime,
                    value => velocity = value).SetEase(x.characterProperty.Dash.Attack.KnockbackEase);
            }).AddTo(this);
        }

        private void Update()
        {
            characterController.Move(velocity * Time.deltaTime);
        }
    }
}
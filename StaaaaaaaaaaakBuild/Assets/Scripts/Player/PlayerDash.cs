using System;
using DG.Tweening;
using UniRx;
using UnityEngine;

namespace StackBuild
{
    public class PlayerDash : MonoBehaviour
    {
        [SerializeField] private InputSender inputSender;

        [SerializeField] private float DashAcceleration = 10.0f;
        [SerializeField] private float DashMaxSpeed = 10.0f;
        [SerializeField] private float DashTimeToMaxSpeed = 0.5f;
        [SerializeField] private float DashDeceleratingSpeed = 0.5f;

        [SerializeField] private Ease DashEaseOfAcceleration = Ease.OutCirc;
        [SerializeField] private Ease DashEaseOfDeceleration = Ease.OutCirc;


        private Vector3 velocity;

        private void Start()
        {
            inputSender.Dash.Subscribe(x =>
            {
                Dash();
            }).AddTo(this);
        }

        void Dash()
        {
            var dir = new Vector3(inputSender.Move.Value.x, 0.0f, inputSender.Move.Value.y).normalized;

            DOVirtual.Vector3(Vector3.zero, dir * DashMaxSpeed, DashTimeToMaxSpeed, value => {
                transform.position += value;
            }).SetEase(DashEaseOfAcceleration).
                OnComplete(() => {
                DOVirtual.Vector3(dir * DashMaxSpeed, Vector3.zero, DashDeceleratingSpeed, value =>
                {
                    transform.position += value;
                }).SetEase(DashEaseOfDeceleration);
            }).Play();
        }
    }
}
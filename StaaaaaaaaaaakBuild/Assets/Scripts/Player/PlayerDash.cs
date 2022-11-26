using System;
using DG.Tweening;
using UniRx;
using UnityEngine;

namespace StackBuild
{
    public class PlayerDash : MonoBehaviour
    {
        [SerializeField] private InputSender inputSender;


        [SerializeField] private float DashMaxSpeed = 0.1f;
        [SerializeField] private float DashTimeToMaxSpeed = 0.2f;
        [SerializeField] private float DashDeceleratingSpeed = 0.6f;

        [SerializeField] private Ease DashEaseOfAcceleration = Ease.OutCirc;
        [SerializeField] private Ease DashEaseOfDeceleration = Ease.OutQuad;

        [SerializeField] private float DashCoolTime = 1.0f;

        private void Start()
        {
            inputSender.Dash.Where(x => x).ThrottleFirst(TimeSpan.FromSeconds(DashCoolTime)).Subscribe(_ =>
            {
                Dash();
            }).AddTo(this);
        }

        void Dash()
        {
            var moveDir = CreateMoveDirection().normalized * DashMaxSpeed;

            var sequence = DOTween.Sequence();

            //加速する
            sequence.Append(DOVirtual.Vector3(Vector3.zero, moveDir, DashTimeToMaxSpeed,
                value => transform.position += value).SetEase(DashEaseOfAcceleration)).SetLink(gameObject);

            //加速度を0に戻す
            sequence.Append(DOVirtual
                .Vector3(moveDir, Vector3.zero, DashDeceleratingSpeed, value => transform.position += value)
                .SetEase(DashEaseOfDeceleration)).SetLink(gameObject);

            sequence.Play();
        }

        Vector3 CreateMoveDirection()
        {
            //いちおう相談
            var dir = new Vector3(inputSender.Move.Value.x, 0.0f, inputSender.Move.Value.y);
            if (dir.sqrMagnitude <= 0.0f)
            {
                dir = -transform.forward;
                dir.y = 0.0f;
            }

            return dir;
        }
    }
}
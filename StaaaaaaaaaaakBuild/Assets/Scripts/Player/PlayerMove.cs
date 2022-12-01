using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;

namespace StackBuild
{
    public class PlayerMove : MonoBehaviour
    {
        [SerializeField] private InputSender inputSender;
        [SerializeField] private PlayerProperty playerProperty;
        [SerializeField] private NetworkObject networkObject;

        private CharacterProperty property
        {
            get
            {
                return playerProperty.characterProperty;
            }
        }

        private Quaternion targetLook;

        private Vector3 velocity = Vector3.zero;

        private Quaternion startRotation = Quaternion.identity;

        private bool hit = false;

        private Rigidbody rb;

        private float enemyWeight = 0.0f;

        private const float MaxPowerAndWeight = 1000.0f;
        [field: SerializeField] public float Power { get; private set; } = MaxPowerAndWeight;
        [field: SerializeField] public float Weight { get; private set; } = MaxPowerAndWeight;

        private void Start()
        {
            TryGetComponent(out rb);
            startRotation = transform.rotation;
        }

        private void Update()
        {
            if (!networkObject.IsOwner)
                return;

            MoveVelocity();
            LookForward();
            Slope();

            rb.velocity = velocity;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!collision.collider.CompareTag("P1") && !collision.collider.CompareTag("P2"))
                return;

            if (!collision.collider.TryGetComponent(out PlayerMove move))
                return;

            enemyWeight = move.Weight;
            hit = true;
        }

        private void OnCollisionExit(Collision collision)
        {
            hit = false;
        }

        void MoveVelocity()
        {
            // hit時重さと力で加速度を変える
            if (hit)
            {
                var diff = enemyWeight - Power;
                if (diff >= 0.0f)
                    velocity *= 1.0f - (diff / MaxPowerAndWeight);
            }

            //移動方向取得
            var dir = CreateMoveDirection();

            //移動減衰
            if (Mathf.Abs(dir.x) <= 0.0f)
                velocity.x *= property.Move.Deceleration;

            if (Mathf.Abs(dir.z) <= 0.0f)
                velocity.z *= property.Move.Deceleration;

            //移動
            if (dir.sqrMagnitude > 0.0f)
            {
                velocity += dir * (property.Move.Acceleration * Time.deltaTime);
            }

            //最高速超えないようにする
            if (velocity.sqrMagnitude >=
                property.Move.MaxSpeed * property.Move.MaxSpeed)
                velocity = velocity.normalized * property.Move.MaxSpeed;
        }

        void LookForward()
        {
            if (CreateMoveDirection().sqrMagnitude > 0.0f && velocity.sqrMagnitude > 0.0f)
                targetLook = Quaternion.LookRotation(-velocity);

            transform.rotation = Quaternion.Lerp(transform.rotation,
                targetLook,
                property.Move.LookForwardTime * Time.deltaTime);
        }

        void Slope()
        {
            var raito = velocity.sqrMagnitude /
                        (property.Move.MaxSpeed * property.Move.MaxSpeed);

            var rotation = transform.rotation;
            var target = Quaternion.AngleAxis(property.Move.SlopeAngle * raito, -transform.right) * rotation;

            transform.rotation = Quaternion.Lerp(rotation, target, property.Move.SlopeTime * Time.deltaTime);
        }

        Vector3 CreateMoveDirection()
        {
            return new Vector3(inputSender.Move.Value.x, 0.0f, inputSender.Move.Value.y);
        }
    }
}

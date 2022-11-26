using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;

namespace StackBuild
{
    public class PlayerMove : MonoBehaviour
    {
        [SerializeField] private InputSender inputSender;
        [SerializeField] private PlayerProperty playerProperty;

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

        private void Start()
        {
            startRotation = transform.rotation;
        }

        private void Update()
        {
            Move();
            LookForward();
            Slope();
        }

        void Move()
        {
            var dir = CreateMoveDirection();

            //移動
            if (Mathf.Abs(dir.x) <= 0.0f)
                velocity.x *= property.Deceleration;

            if (Mathf.Abs(dir.z) <= 0.0f)
                velocity.z *= property.Deceleration;

            if (dir.sqrMagnitude > 0.0f)
            {
                velocity += dir * (property.Acceleration * Time.deltaTime);
            }

            if (velocity.sqrMagnitude >=
                property.MaxSpeed * property.MaxSpeed)
                velocity = velocity.normalized * property.MaxSpeed;

            transform.position += velocity * Time.deltaTime;
        }

        void LookForward()
        {
            if (Mathf.Abs(CreateMoveDirection().sqrMagnitude) > 0.0f)
                targetLook = Quaternion.LookRotation(-velocity);

            transform.rotation = Quaternion.Lerp(transform.rotation,
                targetLook,
                property.LookForwardTime * Time.deltaTime);
        }

        void Slope()
        {
            var raito = velocity.sqrMagnitude /
                        (property.MaxSpeed * property.MaxSpeed);

            var rotation = transform.rotation;
            var target = Quaternion.AngleAxis(property.SlopeAngle * raito, -transform.right) * rotation;

            rotation = Quaternion.Lerp(rotation, target, property.SlopeTime * Time.deltaTime);
            transform.rotation = rotation;

        }

        Vector3 CreateMoveDirection()
        {
            return new Vector3(inputSender.Move.Value.x, 0.0f, inputSender.Move.Value.y);
        }
    }
}

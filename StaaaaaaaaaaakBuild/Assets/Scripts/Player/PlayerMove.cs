using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StackBuild
{
    public class PlayerMove : MonoBehaviour
    {
        [SerializeField] private InputSender inputSender;
        private Vector3 velocity = Vector3.zero;

        [SerializeField] private float acceleration = 1000.0f;
        [SerializeField] private float deceleration = 0.99f;
        [SerializeField] private float maxSpeed = 20.0f;

        private void Update()
        {
            Move();
        }

        void Move()
        {
            var dir = new Vector3(inputSender.Move.Value.x, 0.0f, inputSender.Move.Value.y);

            //移動
            if (Mathf.Abs(dir.x) <= 0.0f)
                velocity.x *= deceleration;

            if (Mathf.Abs(dir.z) <= 0.0f)
                velocity.z *= deceleration;

            if (dir.sqrMagnitude > 0.0f)
            {
                velocity += dir * (acceleration * Time.deltaTime);
            }

            if (velocity.sqrMagnitude >= maxSpeed * maxSpeed)
                velocity = velocity.normalized * maxSpeed;

            transform.position += velocity * Time.deltaTime;
        }
    }
}

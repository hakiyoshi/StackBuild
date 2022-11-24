using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StackBuild
{
    public class PlayerMove : MonoBehaviour
    {
        [SerializeField] private InputSender inputSender;
        [SerializeField] private PlayerProperty playerProperty;

        private Vector3 velocity = Vector3.zero;

        private void Update()
        {
            Move();
        }

        void Move()
        {
            var dir = new Vector3(inputSender.Move.Value.x, 0.0f, inputSender.Move.Value.y);

            //移動
            if (Mathf.Abs(dir.x) <= 0.0f)
                velocity.x *= playerProperty.characterProperty.Deceleration;

            if (Mathf.Abs(dir.z) <= 0.0f)
                velocity.z *= playerProperty.characterProperty.Deceleration;

            if (dir.sqrMagnitude > 0.0f)
            {
                velocity += dir * (playerProperty.characterProperty.Acceleration * Time.deltaTime);
            }

            if (velocity.sqrMagnitude >=
                playerProperty.characterProperty.MaxSpeed * playerProperty.characterProperty.MaxSpeed)
                velocity = velocity.normalized * playerProperty.characterProperty.MaxSpeed;

            transform.position += velocity * Time.deltaTime;
        }
    }
}

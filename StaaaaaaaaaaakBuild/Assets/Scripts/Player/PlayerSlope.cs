using System;
using UnityEngine;

namespace StackBuild
{
    public class PlayerSlope : MonoBehaviour
    {
        [SerializeField] private PlayerProperty playerProperty;
        [SerializeField] private InputSender inputSender;

        private CharacterProperty property
        {
            get
            {
                return playerProperty.characterProperty;
            }
        }

        private void Update()
        {
            Slope();
        }

        void Slope()
        {
            var velocity = CreateMoveDirection();
            const float size = 1.414213f;// (1,0,1)の長さ

            //傾き率を計算
            var raito = velocity.sqrMagnitude / (size * size);

            //傾く方向を計算
            var rotation = transform.rotation;
            var target = Quaternion.AngleAxis(property.Move.SlopeAngle * raito, -transform.right) * rotation;

            //傾けぇ
            transform.rotation = Quaternion.Lerp(rotation, target, property.Move.SlopeTime * Time.deltaTime);
        }

        Vector3 CreateMoveDirection()
        {
            return new Vector3(inputSender.Move.Value.x, 0.0f, inputSender.Move.Value.y);
        }
    }
}
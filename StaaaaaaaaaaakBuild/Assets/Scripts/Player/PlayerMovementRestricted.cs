using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace StackBuild
{
    public class PlayerMovementRestricted : MonoBehaviour
    {
        [SerializeField] private Vector3 firstPoint;
        [SerializeField] private Vector3 secondPoint;
        [SerializeField] private float radius;

        private void LateUpdate()
        {
            var posi = transform.position;
            var capsuleVec = secondPoint - firstPoint;
            var fpVec = posi - firstPoint;
            var distance = capsuleVec * (Vector3.Dot(capsuleVec.normalized, fpVec) / capsuleVec.magnitude) - fpVec;

            if (Vector3.Dot(fpVec.normalized, capsuleVec.normalized) <= 0.0f)
            {
                distance = firstPoint - posi;
            }
            else if(Vector3.Dot((posi - secondPoint).normalized, -capsuleVec.normalized) <= 0.0f)
            {
                distance = secondPoint - posi;
            }

            if (distance.sqrMagnitude > radius * radius)//半径より遠い場合
            {
                posi = (posi + distance) - distance.normalized * radius;
            }
            transform.position = posi;
        }
    }
}
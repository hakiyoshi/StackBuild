using System;
using UnityEngine;

namespace StackBuild
{
    public class Canon : MonoBehaviour
    {
        public float ShootPower;
        public float ShootAngle;
        [SerializeField] private Transform shootPosition;

        private void Start()
        {

        }

        private void OnDrawGizmos()
        {

        }
    }
}
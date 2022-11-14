using System;
using UniRx;
using UnityEngine;

namespace StackBuild
{
    // Require collision
    public class VacuumArea : MonoBehaviour
    {
        [SerializeField] private IReactiveProperty<Vector3> playerVelocityProperty;
        [SerializeField] private IReactiveProperty<bool> playerVacuumInputProperty;
        [SerializeField] private float vacuumPower;
        [SerializeField] private float vacuumDecelerationDistance;
        [SerializeField] private Vector3 vacuumColliderSize;
        [SerializeField] private Transform vacuumPoint;

        private Vector3 playerVelocity = Vector3.zero;
        private bool playerVacuumInput = false;

        private void Start()
        {
            playerVelocityProperty.Subscribe((v) => { playerVelocity = v;});
            playerVacuumInputProperty.Subscribe((b) => { playerVacuumInput = b;});
        }

        public void Vacuum(Rigidbody rb)
        {
            if (!playerVacuumInput) return;

            var center = vacuumPoint.position;
            var sub = center - rb.transform.position;
            var power = sub * vacuumPower * Time.deltaTime;
            var distance = sub.magnitude;

            if (distance < vacuumDecelerationDistance) power *= 0.1f;

            rb.AddForce(playerVelocity * Time.deltaTime);
            rb.AddForceAtPosition(power, center, ForceMode.VelocityChange);
        }
    }
}

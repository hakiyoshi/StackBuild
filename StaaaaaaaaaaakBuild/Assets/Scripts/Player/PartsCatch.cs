using System;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace StackBuild
{
    public class PartsCatch : MonoBehaviour
    {
        [SerializeField] private InputSender inputSender;
        [SerializeField] private PlayerProperty playerProperty;

        private void OnTriggerStay(Collider other)
        {
            if (!other.CompareTag("Parts"))
                return;

            if(inputSender.Catch.Value && other.TryGetComponent(out Rigidbody rb))
                Catch(rb);
        }

        private void Catch(Rigidbody rb)
        {
            var parentPosition = transform.parent.position;

            var center = parentPosition + playerProperty.CatchupOffsetPosition;
            var sub = center - rb.transform.position;

            rb.AddForceAtPosition(sub * (playerProperty.CatchupPower * Time.deltaTime), center, ForceMode.VelocityChange);

            var magnitude = sub.magnitude;
            if (magnitude < transform.parent.position.y)
            {
                rb.velocity *= (magnitude / transform.parent.position.y);
            }
        }
    }
}
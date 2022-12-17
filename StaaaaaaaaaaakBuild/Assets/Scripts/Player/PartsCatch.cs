using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using Unity.Netcode;
using UnityEngine;

namespace StackBuild
{
    public class PartsCatch : MonoBehaviour
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

            var center = parentPosition + property.Catch.CatchupOffsetPosition;
            var sub = center - rb.transform.position;

            rb.AddForceAtPosition(sub * (property.Catch.CatchupPower * Time.deltaTime), center, ForceMode.Impulse);

            var magnitude = sub.magnitude;
            if (magnitude < parentPosition.y)
            {
                rb.velocity *= (magnitude / parentPosition.y);
            }
        }
    }
}
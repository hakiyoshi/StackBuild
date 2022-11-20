﻿using System;
using Cysharp.Threading.Tasks;
using UniRx;
using Unity.Netcode;
using UnityEngine;

namespace StackBuild
{
    public class PartsCatch : NetworkBehaviour
    {
        [SerializeField] private InputSender inputSender;
        [SerializeField] private PlayerProperty playerProperty;

        private bool isCatch = false;

        [ServerRpc]
        void CatchServerRpc(bool isCatchFlag)
        {
            isCatch = isCatchFlag;
            CatchClientRpc(isCatchFlag);
        }

        [ClientRpc]
        void CatchClientRpc(bool isCatchFlag)
        {
            if (IsOwner)
                return;

            isCatch = isCatchFlag;
        }

        private void Start()
        {
            inputSender.Catch.Subscribe(x =>
            {
                isCatch = x;
            }).AddTo(this);
        }

        private void OnTriggerStay(Collider other)
        {
            if (!other.CompareTag("Parts"))
                return;

            if(isCatch && other.TryGetComponent(out Rigidbody rb))
                Catch(rb);
        }

        private void Catch(Rigidbody rb)
        {
            var parentPosition = transform.parent.position;

            var center = parentPosition + playerProperty.CatchupOffsetPosition;
            var sub = center - rb.transform.position;

            rb.AddForceAtPosition(sub * (playerProperty.CatchupPower * Time.deltaTime), center, ForceMode.Impulse);

            var magnitude = sub.magnitude;
            if (magnitude < parentPosition.y)
            {
                rb.velocity *= (magnitude / parentPosition.y);
            }
        }
    }
}
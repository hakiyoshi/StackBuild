using System.Threading;
using UniRx;
using UniRx.Triggers;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.EventSystems;

namespace StackBuild
{
    [RequireComponent(typeof(PartsCore))]
    [RequireComponent(typeof(Rigidbody))]
    public class PartsPhysics : MonoBehaviour
    {
        [field: SerializeField] public PartsCore PartsCore { get; private set; }

        private Rigidbody rb;

        private void Awake()
        {
            TryGetComponent(out rb);
        }

        private void Start()
        {
            PartsCore.isActive
                .Subscribe(SetActive).AddTo(this);
        }

        [ServerRpc(RequireOwnership = true)]
        private void SetActive(bool isActive) => SetActiveImpl(isActive);

        [ClientRpc]
        private void SetActiveImpl(bool isActive)
        {
            if (isActive)
            {
                rb.WakeUp();
                rb.useGravity = true;
            }
            else
            {
                rb.useGravity = false;
                rb.Sleep();
            }
        }

            [ServerRpc(RequireOwnership = true)]
        public void Shoot(Vector3 pos, Vector3 force, ForceMode forceMode = ForceMode.Force)
        {
            rb.velocity = Vector3.zero;
            rb.position = pos;
            PartsCore.isActive.Value = true;
            rb.AddForce(force, forceMode);
        }
    }
}
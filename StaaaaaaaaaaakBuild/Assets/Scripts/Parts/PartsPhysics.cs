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
    [RequireComponent(typeof(ClientNetworkTransform))]
    [RequireComponent(typeof(Rigidbody))]
    public class PartsPhysics : MonoBehaviour
    {
        [SerializeField] private PartsCore partsCore;
        public PartsCore PartsCore => partsCore;

        private Rigidbody rb;

        private void Awake()
        {
            TryGetComponent(out rb);
        }

        private void Start()
        {
            partsCore.IsActive
                .Subscribe((isActive) =>
                {
                    if (isActive)
                    {
                        rb.WakeUp();
                    }
                    else
                    {
                        rb.Sleep();
                    }
                }).AddTo(this);
        }

        public void Teleport(Vector3 position)
        {
            rb.velocity = Vector3.zero;
            rb.position = position;
        }

        public void AddForce(Vector3 force, ForceMode forceMode = ForceMode.Force)
        {
            rb.AddForce(force, forceMode);
        }
    }
}
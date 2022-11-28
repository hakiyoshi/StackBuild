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
    public class PartsPhysics : NetworkBehaviour
    {
        [SerializeField] private PartsCore partsCore;
        [SerializeField] private Rigidbody rigidbody;

        public void Start()
        {
            partsCore.IsActive
                .Subscribe((isActive) =>
                {
                    if (isActive)
                    {
                        rigidbody.isKinematic = false;
                        rigidbody.WakeUp();
                    }
                    else
                    {
                        rigidbody.isKinematic = true;
                        rigidbody.Sleep();
                    }
                }).AddTo(this);

            this.OnCollisionStayAsObservable()
                .Where(x => x.gameObject.CompareTag("Player"))
                //.Select(x => x.gameObject.GetComponent(<Player>())
                .Subscribe((player) =>
                {
                    // player.Vacuum(rigidbody);
                }).AddTo(this);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
        }

        public override void OnGainedOwnership()
        {
            base.OnGainedOwnership();
        }

        public override void OnLostOwnership()
        {
            base.OnLostOwnership();
        }
    }
}
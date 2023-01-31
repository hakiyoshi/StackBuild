using UniRx;
using UniRx.Triggers;
using Unity.Netcode;
using UnityEngine;

namespace StackBuild
{
    [RequireComponent(typeof(PartsCore))]
    public class PartsNetworkSync : NetworkBehaviour
    {
        [SerializeField] private float extrapolateFromSeconds = 0.5f;

        private PartsCore partsCore = null;
        private Rigidbody rb = null;
        private NetworkObject networkObject;
        private NetworkVariable<bool> isActiveNetworkVariable = new();
        private NetworkVariable<PartsId> partsIdNetworkVariable = new();

        private void Awake()
        {
            TryGetComponent(out partsCore);
            TryGetComponent(out rb);
            TryGetComponent(out networkObject);
        }

        public override void OnNetworkSpawn()
        {
            partsCore.isActive
                .Where(_ => IsOwner && IsSpawned)
                .Subscribe(isActive =>
            {
                isActiveNetworkVariable.Value = isActive;
            }).AddTo(this);

            partsCore.partsId
                .Where(_ => IsOwner && IsSpawned)
                .Subscribe(partsId =>
            {
                partsIdNetworkVariable.Value = partsId;
            }).AddTo(this);

            isActiveNetworkVariable.OnValueChanged += ApplyIsActive;
            partsIdNetworkVariable.OnValueChanged += ApplyPartsId;

            partsCore.isActive.Value = isActiveNetworkVariable.Value;
            partsCore.partsId.Value = partsIdNetworkVariable.Value;

            this.OnTriggerEnterAsObservable()
                .Where(col => col.CompareTag("Catch"))
                .Select(col => col.transform.parent.parent.GetComponent<NetworkObject>())
                .Subscribe(player =>
                {
                    if(!player.IsOwner)
                        return;

                    ChangeOwnershipServerRpc(player.OwnerClientId);
                }).AddTo(this);
        }

        public override void OnNetworkDespawn()
        {
            isActiveNetworkVariable.OnValueChanged -= ApplyIsActive;
            partsIdNetworkVariable.OnValueChanged -= ApplyPartsId;
        }

        [ServerRpc(RequireOwnership = false)]
        public void ChangeOwnershipServerRpc(ulong clientId)
        {
            if (!IsServer) return;

            networkObject.ChangeOwnership(clientId);
        }

        [ServerRpc(RequireOwnership = false)]
        public void LostOwnershipServerRpc(ulong clientId)
        {
            if (!IsServer) return;

            networkObject.ChangeOwnership(clientId);
        }

        [ServerRpc(RequireOwnership = false)]
        private void UpdatePositionServerRpc(Vector3 position)
        {
            rb.position = position + rb.velocity * Time.fixedDeltaTime * (extrapolateFromSeconds / 50.0f);
        }

        [ServerRpc(RequireOwnership = false)]
        private void UpdateVelocityServerRpc(Vector3 velocity)
        {
            rb.velocity = velocity;
        }

        private void ApplyIsActive(bool prev, bool current)
        {
            if (IsOwner) return;
            partsCore.isActive.Value = current;
        }

        private void ApplyPartsId(PartsId prev, PartsId current)
        {
            if (IsOwner) return;
            partsCore.partsId.Value = current;
        }
    }
}
using UniRx;
using Unity.Netcode;
using UnityEngine;

namespace StackBuild
{
    [RequireComponent(typeof(PartsCore))]
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshCollider))]
    [RequireComponent(typeof(MeshFilter))]
    public class PartsMesh : MonoBehaviour
    {
        [field: SerializeField]
        public PartsCore PartsCore { get; private set; }

        private MeshRenderer meshRenderer;
        private MeshCollider meshCollider;
        private MeshFilter meshFilter;

        private void Awake()
        {
            TryGetComponent(out meshRenderer);
            TryGetComponent(out meshCollider);
            TryGetComponent(out meshFilter);
        }

        private void Start()
        {
            PartsCore.isActive
                .Subscribe(SetActive).AddTo(this);

            PartsCore.partsId
                .Select(_ => PartsCore.GetPartsData())
                .Subscribe(SetPartsMesh).AddTo(this);
        }

        [ServerRpc(RequireOwnership = true)]
        private void SetActive(bool isActive) => SetActiveClientRpc(isActive);

        [ClientRpc]
        private void SetActiveClientRpc(bool isActive)
        {
            meshRenderer.enabled = isActive;
            meshCollider.enabled = isActive;
        }

        [ServerRpc(RequireOwnership = true)]
        private void SetPartsMesh(PartsData data) => SetPartsMeshClientRpc(data);

        [ClientRpc]
        private void SetPartsMeshClientRpc(PartsData data)
        {
            meshRenderer.sharedMaterial = data.material;
            meshFilter.sharedMesh = data.mesh;
            meshCollider.sharedMesh = data.mesh;
        }
    }
}

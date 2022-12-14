using UniRx;
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
                .Subscribe(SetPartsMesh).AddTo(this);
        }

        private void SetActive(bool isActive)
        {
            meshRenderer.enabled = isActive;
            meshCollider.enabled = isActive;
        }

        private void SetPartsMesh(PartsId id)
        {
            var data = PartsCore.Settings.PartsDataDictionary[id];

            meshRenderer.sharedMaterial = data.material;
            meshFilter.sharedMesh = data.mesh;
            meshCollider.sharedMesh = data.mesh;
        }
    }
}

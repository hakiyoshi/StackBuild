using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using VContainer;

namespace StackBuild
{
    [RequireComponent(typeof(PartsCore))]
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshCollider))]
    [RequireComponent(typeof(MeshFilter))]
    public class PartsMesh : MonoBehaviour
    {
        [SerializeField] private PartsCore partsCore;
        public PartsCore PartsCore => partsCore;

        private MeshRenderer meshRenderer;
        private MeshCollider meshCollider;
        private MeshFilter meshFilter;

        public PartsId ID
        {
            get;
            private set;
        }

        public Material SharedMaterial
        {
            get => meshRenderer.sharedMaterial;
            private set => meshRenderer.sharedMaterial = value;
        }

        public Mesh SharedMesh
        {
            get => meshFilter.sharedMesh;
            private set
            {
                meshFilter.sharedMesh = value;
                meshCollider.sharedMesh = value;
            }
        }

        private void Awake()
        {
            TryGetComponent(out meshRenderer);
            TryGetComponent(out meshCollider);
            TryGetComponent(out meshFilter);
        }

        private void Start()
        {
            partsCore.IsActive
                .Subscribe((isActive) =>
            {
                meshRenderer.enabled = isActive;
                meshCollider.enabled = isActive;
            }).AddTo(this);

            partsCore.PartsID
                .Where(x => x != PartsId.Default)
                .Subscribe(id =>
            {
                var data = partsCore.Settings.PartsDataDictionary[id];

                ID = id;
                SharedMaterial = data.material;
                SharedMesh = data.mesh;
            }).AddTo(this);
        }
    }
}

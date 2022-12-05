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
        [SerializeField] private InGameSettings inGameSettings;
        [SerializeField] private PartsCore partsCore;
        public PartsCore PartsCore => partsCore;

        private MeshRenderer meshRenderer;
        private MeshCollider meshCollider;
        private MeshFilter meshFilter;

        public int MeshId { get; private set; } = -1;
        public string Name { get; private set; } = "";
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
            partsCore.IsActive.Subscribe((isActive) =>
            {
                meshRenderer.enabled = isActive;
                meshCollider.enabled = isActive;
            }).AddTo(this);
        }

        public void SetPartsData(string name)
        {
            var index = inGameSettings.partsMeshList.FindIndex(x => String.Equals(x.name, name));
            if (index == -1) return;
            SetPartsData(index);
        }

        public void SetPartsData(int index)
        {
            if (index < 0 || inGameSettings.partsMeshList.Count <= index) return;

            var data = inGameSettings.partsMeshList[index];

            MeshId = index;
            Name = data.name;
            SharedMaterial = data.material;
            SharedMesh = data.mesh;
        }
    }
}

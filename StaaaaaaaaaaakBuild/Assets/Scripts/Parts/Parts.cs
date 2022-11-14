using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StackBuild
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshCollider))]
    [RequireComponent(typeof(MeshFilter))]
    public class Parts : MonoBehaviour
    {
        private Rigidbody rb;
        private MeshRenderer meshRenderer;
        private MeshCollider meshCollider;
        private MeshFilter meshFilter;

        public Material PartsMaterial
        {
            get => meshRenderer.sharedMaterial;
            set => meshRenderer.sharedMaterial = value;
        }
        public Mesh PartsMesh
        {
            get => meshFilter.sharedMesh;
            set
            {
                meshFilter.sharedMesh = value;
                meshCollider.sharedMesh = value;
            }
        }

        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            meshRenderer = GetComponent<MeshRenderer>();
            meshCollider = GetComponent<MeshCollider>();
            meshFilter = GetComponent<MeshFilter>();
        }

        private void OnCollisionStay(Collision collision)
        {
            if (collision.gameObject.TryGetComponent(out VacuumArea area))
            {
                area.Vacuum(rb);
            }
        }
    }
}

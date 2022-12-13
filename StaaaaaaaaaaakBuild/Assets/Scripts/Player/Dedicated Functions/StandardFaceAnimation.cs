using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

namespace StackBuild
{
    public class StandardFaceAnimation : MonoBehaviour
    {
        [SerializeField] private SkinnedMeshRenderer meshRenderer;

        private int standardFace = (int)FaceType.Normal;

        private Material material
        {
            get { return meshRenderer.material; }
            set { meshRenderer.material = value; }
        }

        private Vector2 Face
        {
            get { return material.GetVector("_Face"); }
            set { material.SetVector("_Face", new Vector4(value.x, value.y)); }
        }

        enum FaceType : int
        {
            Normal,
            Anger,
            Bewilderment,
            xFace,
        }
        private readonly Vector2[] FaceUV = {
            new(0f, 0f),
            new(0.5f, 0f),
            new(0f, 0.8f),
            new(0.5f, 0.8f),
        };

        private void Start()
        {
            standardFace = Random.Range(0, 3);
            Face = FaceUV[standardFace];
        }

        private void Update()
        {

        }
    }
}
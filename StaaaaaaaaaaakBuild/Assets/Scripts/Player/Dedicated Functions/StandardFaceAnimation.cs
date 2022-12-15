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
        private ModelSetup modelSetup;
        private InputSender inputSender => modelSetup.inputSender;
        private PlayerProperty playerProperty => modelSetup.playerProperty;

        private const string FaceName = "_Face";
        int shaderFaceId = -1; //FaceのNameID

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

        private Material material
        {
            get { return meshRenderer.material; }
            set { meshRenderer.material = value; }
        }

        private Vector2 Face
        {
            get { return material.GetVector(shaderFaceId); }
        }

        void SetFace(Vector2 uv)
        {
            material.SetVector(shaderFaceId, new Vector4(uv.x, uv.y));
        }

        void SetFace(int faceTypeId)
        {
            material.SetVector(shaderFaceId, new Vector4(FaceUV[faceTypeId].x, FaceUV[faceTypeId].y));
        }

        void SetFace(FaceType type)
        {
            var uv = FaceUV[(int) type];
            material.SetVector(shaderFaceId, new Vector4(uv.x, uv.y));
        }

        private void Start()
        {
            //モデルセットアップを取得
            transform.parent.TryGetComponent(out modelSetup);

            //マテリアルのSetVectorで使うFaceIdを取得
            shaderFaceId = Shader.PropertyToID(FaceName);

            //最初の表情
            var startFace = Random.Range(0, 3);
            SetFace(startFace);
            DOVirtual.DelayedCall(1.0f, () => SetFace(FaceType.Normal));


        }

        private void Update()
        {

        }
    }
}
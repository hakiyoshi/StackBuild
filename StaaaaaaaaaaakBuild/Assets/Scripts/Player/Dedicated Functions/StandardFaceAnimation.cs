using System;
using DG.Tweening;
using UniRx;
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
        private CharacterProperty property => playerProperty.characterProperty;

        private const string FaceName = "_Face";
        int shaderFaceId = -1; //FaceのNameID

        private int standardFace = (int) FaceType.Normal;

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

            inputSender.Dash.sender.Where(x => x).ThrottleFirst(TimeSpan.FromSeconds(property.Dash.DashCoolTime)).Subscribe(_ =>
            {
                SetFace(FaceType.Anger);

                //ダッシュが終わるタイミングで普通に戻す
                DOVirtual.DelayedCall(property.Dash.DashAccelerationTime + property.Dash.DashDeceleratingTime,
                    () => SetFace(standardFace));
            }).AddTo(this);

            playerProperty.HitDashAttack.Subscribe(x =>
            {
                var faceType = FaceType.Normal;
                var returnTime = 1.0f;

                if (x.playerProperty.characterProperty.Attack.StunTime != 0)
                {
                    //スタンする場合
                    faceType = FaceType.Anger;
                    returnTime = x.playerProperty.characterProperty.Attack.StunTime;
                }
                else
                {
                    //スタンしない場合
                    faceType = FaceType.Bewilderment;
                    returnTime = 1.0f;
                }

                SetFace(faceType);

                //指定時間で普通に戻す
                DOVirtual.DelayedCall(returnTime,
                    () => SetFace(standardFace));
            }).AddTo(this);
        }
    }
}
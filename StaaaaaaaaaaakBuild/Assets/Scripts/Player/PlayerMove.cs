using System;
using DG.Tweening;
using UniRx;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering;

namespace StackBuild
{
    public class PlayerMove : MonoBehaviour
    {
        [SerializeField] private InputSender inputSender;
        [SerializeField] private PlayerProperty playerProperty;
        [SerializeField] private NetworkObject networkObject;

        private CharacterProperty property
        {
            get
            {
                return playerProperty.characterProperty;
            }
        }

        private CharacterController characterController;
        private Vector3 velocity = Vector3.zero;
        private float startY = 20.0f;

        private bool dashHit = false;

        private void Start()
        {
            //キャラクターコントローラー
            if (TryGetComponent(out characterController))
            {
                characterController.detectCollisions = false;
                characterController.radius = property.Model.SphereColliderRadius;
            }

            //コライダーのサイズセット
            if (TryGetComponent(out CapsuleCollider collider))
                collider.radius = property.Model.SphereColliderRadius;

            //開始時のY座標を取得
            startY = transform.position.y;

            //ダッシュ攻撃ヒット時の処理
            playerProperty.HitDashAttack.Subscribe(x =>
            {
                inputSender.Move.isPause = true;
                dashHit = true;
                velocity = Vector3.zero;

                //指定時間後スタンフラグを元に戻す
                Observable.Timer(TimeSpan.FromSeconds(x.characterProperty.Dash.Attack.StunTime)).Subscribe(_ =>
                {
                    inputSender.Move.isPause = false;
                    dashHit = false;
                }).AddTo(this);
            }).AddTo(this);
        }

        private void Update()
        {
            if (!networkObject.IsOwner)
                return;

            MoveVelocity();

            if(!dashHit)
                characterController.Move(velocity * Time.deltaTime);

            //Yを固定する
            var position = transform.position;
            position = new Vector3(position.x, startY, position.z);
            transform.position = position;
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            //何かに当たったらvelocityを0にする
            velocity = Vector3.zero;
        }

        void MoveVelocity()
        {
            //移動方向取得
            var dir = CreateMoveDirection();

            //移動減衰
            if (Mathf.Abs(dir.x) <= 0.0f)
                velocity.x *= property.Move.Deceleration;

            if (Mathf.Abs(dir.z) <= 0.0f)
                velocity.z *= property.Move.Deceleration;

            //移動
            if (dir.sqrMagnitude > 0.0f)
            {
                velocity += dir * (property.Move.Acceleration * Time.deltaTime);
            }

            //最高速超えないようにする
            if (velocity.sqrMagnitude >=
                property.Move.MaxSpeed * property.Move.MaxSpeed)
                velocity = velocity.normalized * property.Move.MaxSpeed;
        }

        Vector3 CreateMoveDirection()
        {
            //ポーズしてたら0で返す
            if(inputSender.Move.isPause)
                return Vector3.zero;

            return new Vector3(inputSender.Move.Value.x, 0.0f, inputSender.Move.Value.y);
        }
    }
}

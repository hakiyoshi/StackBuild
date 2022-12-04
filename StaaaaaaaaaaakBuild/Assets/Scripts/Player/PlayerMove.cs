using System;
using UniRx;
using Unity.Netcode;
using UnityEngine;

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

        private Quaternion targetLook;

        private CharacterController characterController;
        private Vector3 velocity = Vector3.zero;
        private float startY = 20.0f;

        private bool moveHit = false;

        private bool isStun = false;//スタン中か

        private void Start()
        {
            if (TryGetComponent(out characterController))
            {
                characterController.detectCollisions = false;
                characterController.radius = property.Model.SphereColliderRadius;
            }

            if (TryGetComponent(out SphereCollider collider))
                collider.radius = property.Model.SphereColliderRadius;

            startY = transform.position.y;

            targetLook = transform.rotation;

            playerProperty.DashHitAction.Subscribe(x =>
            {
                isStun = true;
                //指定時間後スタンフラグを元に戻す
                Observable.Timer(TimeSpan.FromSeconds(x.StunTime)).Subscribe(_ =>
                {
                    isStun = false;
                }).AddTo(this);
            }).AddTo(this);
        }

        private void Update()
        {
            if (!networkObject.IsOwner)
                return;

            MoveVelocity();
            Hit();
            LookForward();
            Slope();

            if(!moveHit)
                characterController.Move(velocity * Time.deltaTime);

            var position = transform.position;
            position = new Vector3(position.x, startY, position.z);
            transform.position = position;
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
            if (!moveHit && dir.sqrMagnitude > 0.0f)
            {
                velocity += dir * (property.Move.Acceleration * Time.deltaTime);
            }

            //最高速超えないようにする
            if (velocity.sqrMagnitude >=
                property.Move.MaxSpeed * property.Move.MaxSpeed)
                velocity = velocity.normalized * property.Move.MaxSpeed;
        }

        void LookForward()
        {
            // 移動の入力をしているか、移動ベクトルが0じゃないか
            if (CreateMoveDirection().sqrMagnitude > 0.0f && velocity.sqrMagnitude > 0.0f)
                targetLook = Quaternion.LookRotation(-velocity);

            //ターゲットの方向を向く
            transform.rotation = Quaternion.Lerp(transform.rotation,
                targetLook,
                property.Move.LookForwardTime * Time.deltaTime);
        }

        void Slope()
        {
            //傾き率を計算
            var raito = velocity.sqrMagnitude /
                        (property.Move.MaxSpeed * property.Move.MaxSpeed);

            //傾く方向を計算
            var rotation = transform.rotation;
            var target = Quaternion.AngleAxis(property.Move.SlopeAngle * raito, -transform.right) * rotation;

            //傾けぇ
            transform.rotation = Quaternion.Lerp(rotation, target, property.Move.SlopeTime * Time.deltaTime);
        }

        void Hit()
        {
            // //自分のレイヤーを除外して当たり判定処理
            // var layerMask = LayerMask.GetMask("P1", "P2") & ~(1 << gameObject.layer);
            // if (Physics.SphereCast(transform.position, property.Model.SphereColliderRadius, velocity, out RaycastHit raycast,
            //         (velocity * Time.deltaTime).magnitude + 0.2f,
            //         layerMask))
            // {
            //     //当たったら座標を強制補完する
            //     moveHit = true;
            //     transform.position = raycast.point +
            //                          (new Vector3(raycast.normal.x, 0f, raycast.normal.z) *
            //                           (raycast.distance + property.Model.SphereColliderRadius));
            //     velocity = Vector3.zero;
            //
            //     Debug.Log($"{raycast.distance}の時　{raycast.normal}");
            // }
            // else
            // {
            //     moveHit = false;
            // }
        }

        Vector3 CreateMoveDirection()
        {
            if(isStun)
                return Vector3.zero;

            return new Vector3(inputSender.Move.Value.x, 0.0f, inputSender.Move.Value.y);
        }
    }
}

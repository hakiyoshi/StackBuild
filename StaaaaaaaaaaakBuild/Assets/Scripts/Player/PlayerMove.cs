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

        private Vector3 velocity = Vector3.zero;

        private bool hit = false;

        private void Start()
        {
            if (TryGetComponent(out SphereCollider collider))
                collider.radius = property.Model.SphereColliderRadius;

            targetLook = transform.rotation;
        }

        private void Update()
        {
            if (!networkObject.IsOwner)
                return;

            MoveVelocity();
            Hit();
            LookForward();
            Slope();

            if(!hit)
                transform.position += velocity * Time.deltaTime;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, 3.5f);
            Gizmos.color = Color.white;
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
            //自分のレイヤーを除外して当たり判定処理
            var layerMask = LayerMask.GetMask("P1", "P2") & ~(1 << gameObject.layer);
            if (Physics.SphereCast(transform.position, property.Model.SphereColliderRadius, velocity.normalized, out RaycastHit raycast,
                    (velocity * Time.deltaTime).magnitude + 0.1f,
                    layerMask))
            {
                //当たったら座標を強制補完する
                hit = true;
                transform.position = raycast.point +
                                     (new Vector3(raycast.normal.x, 0f, raycast.normal.z) *
                                      (raycast.distance + property.Model.SphereColliderRadius));
            }
            else
            {
                hit = false;
            }
        }

        Vector3 CreateMoveDirection()
        {
            return new Vector3(inputSender.Move.Value.x, 0.0f, inputSender.Move.Value.y);
        }
    }
}

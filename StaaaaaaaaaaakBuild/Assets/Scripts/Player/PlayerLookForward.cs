using System;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;
using Observable = UniRx.Observable;

namespace StackBuild
{
    public class PlayerLookForward : MonoBehaviour
    {
        [SerializeField] private PlayerProperty playerProperty;
        [SerializeField] private InputSender inputSender;

        private Quaternion targetLook;
        private bool dashHit = false;

        private CharacterProperty property
        {
            get
            {
                return playerProperty.characterProperty;
            }
        }

        private void Start()
        {
            //初期回転を取得
            targetLook = transform.rotation;

            //ダッシュ攻撃が当たったら
            playerProperty.HitDashAttack.Subscribe(x =>
            {
                dashHit = true;

                //指定時間後スタンフラグを元に戻す
                Observable.Timer(TimeSpan.FromSeconds(x.characterProperty.Dash.Attack.StunTime)).Subscribe(_ =>
                {
                    dashHit = false;
                }).AddTo(this);
            }).AddTo(this);
        }

        private void Update()
        {
            if(!dashHit)
                LookForward();
        }

        void LookForward()
        {
            // 移動の入力をしているか、移動ベクトルが0じゃないか
            var velocity = CreateMoveDirection();
            if (velocity.sqrMagnitude > 0.0f)
                targetLook = Quaternion.LookRotation(-velocity);

            //ターゲットの方向を向く
            transform.rotation = Quaternion.Lerp(transform.rotation,
                targetLook,
                property.Move.LookForwardTime * Time.deltaTime);
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
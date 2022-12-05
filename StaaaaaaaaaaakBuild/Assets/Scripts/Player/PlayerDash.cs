using System;
using System.Numerics;
using DG.Tweening;
using UniRx;
using Unity.Netcode;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace StackBuild
{
    public class PlayerDash : NetworkBehaviour
    {
        [SerializeField] private InputSender inputSender;
        [SerializeField] private PlayerProperty playerProperty;
        private ParticleSystem dashParticle;
        private CharacterController characterController;

        private Sequence dashSequence = null;

        private Vector3 velocity = Vector3.zero;

        private bool moveHit = false;

        private CharacterProperty property
        {
            get
            {
               return playerProperty.characterProperty;
            }
        }

        [ServerRpc]
        void DashServerRpc()
        {
            if (!IsServer)
                return;

            DashClientRpc();
        }

        [ClientRpc]
        void DashClientRpc()
        {
            if (IsOwner)
                return;

            DashEffect();
        }

        private void Start()
        {
            TryGetComponent(out characterController);

                //エフェクトオブジェクトを自動生成
            dashParticle = Instantiate(property.Dash.DashEffectPrefab, transform).GetComponent<ParticleSystem>();

            //座標
            dashParticle.transform.localScale = property.Dash.DashEffectMaxScale;

            //ParticleSystemを動的に書き換え
            ParticleSystemSetting();

            dashParticle.Stop(true);

            inputSender.Dash.sender.Where(x => x).ThrottleFirst(TimeSpan.FromSeconds(property.Dash.DashCoolTime)).Subscribe(_ =>
            {
                if (IsSpawned && !IsOwner)
                    return;

                if (!moveHit)
                    DashMove();

                DashEffect();

                DashServerRpc();
            }).AddTo(this);

            playerProperty.DashHitAction.Subscribe(x =>
            {
                //ダッシュを強制終了させる
                dashSequence.Kill(true);
                inputSender.Dash.isPause = true;

                //指定時間経過後フラグを元に戻す
                Observable.Timer(TimeSpan.FromSeconds(x.characterProperty.Dash.Attack.StunTime)).Subscribe(_ =>
                {
                    inputSender.Dash.isPause = false;
                }).AddTo(this);
            }).AddTo(this);
        }

        private void Update()
        {
            if (!moveHit)
                characterController.Move(velocity * Time.deltaTime);
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (velocity.sqrMagnitude <= 0.0f)
                return;

            //ヒット時の相手にヒット情報を送る
            if (hit.collider.TryGetComponent(out PlayerDash playerDash))
                playerDash.playerProperty.DashHitAction.OnNext(playerProperty);

            //当たったらその場でダッシュを止める
            velocity = Vector3.zero;
            dashSequence.Kill(true);
        }

        void DashMove()
        {
            var moveDir = CreateMoveDirection().normalized * property.Dash.DashMaxSpeed;

            if(dashSequence != null && dashSequence.active)
                dashSequence.Kill(true);

            dashSequence = DOTween.Sequence().SetLink(gameObject);

            //加速する
            dashSequence.Append(DOVirtual.Vector3(Vector3.zero, moveDir, property.Dash.DashAccelerationTime,
                value => velocity = value).SetEase(property.Dash.DashEaseOfAcceleration));

            //加速度を0に戻す
            dashSequence.Append(DOVirtual
                .Vector3(moveDir, Vector3.zero, property.Dash.DashDeceleratingTime, value => velocity = value)
                .SetEase(property.Dash.DashEaseOfDeceleration));

            dashSequence.OnKill(() =>
            {
                dashSequence = null;
                velocity = Vector3.zero;
            });
        }

        void DashEffect()
        {
            var sequence = DOTween.Sequence().SetLink(gameObject);

            //加速する
            sequence.Append(DOVirtual.DelayedCall(property.Dash.DashAccelerationTime, () => { }));

            //エフェクト出現
            sequence.Join(EffectSizeAnimation(property.Dash.DashEffectAppearanceTime, property.Dash.DashEffectMaxScale,
                property.Dash.DashEaseOfAcceleration));

            //加速度を0に戻す
            sequence.Append(DOVirtual.DelayedCall(property.Dash.DashDeceleratingTime, () => { }));

            //エフェクト消滅
            sequence.Join(EffectSizeAnimation(property.Dash.DashEffectExitTime, property.Dash.DashEffectMinScale,
                property.Dash.DashEaseOfDeceleration));

            //イベント追加
            sequence.OnStart(() => dashParticle.Play());
            sequence.OnKill(() => dashParticle.Stop());
        }

        //移動方向
        Vector3 CreateMoveDirection()
        {
            var dir = new Vector3(inputSender.Move.Value.x, 0.0f, inputSender.Move.Value.y);
            if (dir.sqrMagnitude <= 0.0f)
            {
                dir = -transform.forward;
                dir.y = 0.0f;
            }

            return dir;
        }

        //パーティクルの初期化
        void ParticleSystemSetting()
        {
            var main = dashParticle.main;
            var fullTime = property.Dash.DashAccelerationTime + property.Dash.DashDeceleratingTime;

            //ライフタイムセット
            main.startLifetime = fullTime;

            //ライフタイムに合わせてカラーの変更
            var colorOverLifetime = dashParticle.colorOverLifetime;
            colorOverLifetime.enabled = true;
            var grad = new Gradient();
            grad.SetKeys(new GradientColorKey[]
            {
                new(Color.white, 0.0f),
                new(Color.white, 1.0f)
            }, new GradientAlphaKey[]
            {
                new(1.0f, 0.0f),
                new(1.0f, property.Dash.DashEffectAppearanceTime / fullTime),
                new(0.0f, 1.0f)
            });
            colorOverLifetime.color = grad;
        }

        //エフェクトのサイズアニメーション
        private Sequence EffectSizeAnimation(float time, Vector3 scale, Ease ease)
        {
            var sequence = DOTween.Sequence().Pause();
            sequence.Join(dashParticle.transform.DOScale(scale, time)
                .SetEase(ease));
            return sequence;
        }
    }
}
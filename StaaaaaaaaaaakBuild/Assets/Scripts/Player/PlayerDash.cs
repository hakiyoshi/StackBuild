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

        private Vector3 velocity = Vector3.zero;

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
            //エフェクトオブジェクトを自動生成
            dashParticle = Instantiate(property.Dash.DashEffectPrefab, transform).GetComponent<ParticleSystem>();

            //座標
            dashParticle.transform.localScale = property.Dash.DashEffectMaxScale;

            //ParticleSystemを動的に書き換え
            ParticleSystemSetting();

            dashParticle.Stop(true);

            inputSender.Dash.Where(x => x).ThrottleFirst(TimeSpan.FromSeconds(property.Dash.DashCoolTime)).Subscribe(_ =>
            {
                if (IsSpawned && !IsOwner)
                    return;

                DashMove();
                DashEffect();

                DashServerRpc();
            }).AddTo(this);
        }

        private void Update()
        {
            transform.position += velocity * Time.deltaTime;
        }

        void DashMove()
        {
            var moveDir = CreateMoveDirection().normalized * property.Dash.DashMaxSpeed;

            var sequence = DOTween.Sequence().SetLink(gameObject);

            //加速する
            sequence.Append(DOVirtual.Vector3(Vector3.zero, moveDir, property.Dash.DashAccelerationTime,
                value => velocity = value).SetEase(property.Dash.DashEaseOfAcceleration));

            //加速度を0に戻す
            sequence.Append(DOVirtual
                .Vector3(moveDir, Vector3.zero, property.Dash.DashDeceleratingTime, value => velocity = value)
                .SetEase(property.Dash.DashEaseOfDeceleration));

            sequence.Play();
        }

        void DashEffect()
        {
            var sequence = DOTween.Sequence().SetLink(gameObject);

            //加速する
            sequence.Append(DOVirtual.DelayedCall(property.Dash.DashAccelerationTime, () => { }));

            //エフェクト出現
            sequence.Join(EffectSizeAnimation(property.Dash.DashEffectAppearanceTime, property.Dash.DashEffectMaxScale, 0.0f, 1.0f,
                property.Dash.DashEaseOfAcceleration));

            //加速度を0に戻す
            sequence.Append(DOVirtual.DelayedCall(property.Dash.DashDeceleratingTime, () => { }));

            //エフェクト消滅
            sequence.Join(EffectSizeAnimation(property.Dash.DashEffectExitTime, property.Dash.DashEffectMinScale, 1.0f, 0.0f,
                property.Dash.DashEaseOfDeceleration));

            //イベント追加
            sequence.OnStart(() => dashParticle.Play());
            sequence.OnComplete(() => dashParticle.Stop());
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


            //ライフタイムに合わせてサイズの変更
            /*
             var curve = new AnimationCurve();
            curve.AddKey(0.0f, 0.0f);
            curve.AddKey(property.Dash.DashEffectAppearanceTime / fullTime, 1.0f);
            curve.AddKey(property.Dash.DashAccelerationTime / fullTime, 1.0f);
            curve.AddKey(1.0f, 0.0f);
            var sizeOverLifetime = dashParticle.sizeOverLifetime;
            sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1.5f, curve);
            sizeOverLifetime.enabled = true;
            */
        }

        //エフェクトのサイズアニメーション
        private Sequence EffectSizeAnimation(float time, Vector3 scale, float startAlfa, float endAlfa, Ease ease)
        {
            var sequence = DOTween.Sequence().Pause();
            sequence.Join(dashParticle.transform.DOScale(scale, time)
                .SetEase(ease));
            return sequence;
        }
    }
}
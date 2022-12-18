using System;
using DG.Tweening;
using StackBuild.Audio;
using UniRx;
using Unity.Netcode;
using UnityEngine;

namespace StackBuild
{
    public class CatchAnimation : NetworkBehaviour
    {
        [SerializeField] private InputSender inputSender;
        [SerializeField] private PlayerProperty playerProperty;
        [SerializeField] private Transform CatchEffectObject;

        [SerializeField] private AudioCue catchCue;
        [SerializeField] private AudioCue catchCancelCue;
        [SerializeField] private AudioSourcePool pool;

        private CharacterProperty property
        {
            get
            {
                return playerProperty.characterProperty;
            }
        }

        private Vector3 startScale;

        //サーバーにキャッチした事を伝える
        [ServerRpc]
        void CatchServerRpc(bool isCatchFlag)
        {
            if (!IsServer)
                return;

            inputSender.Catch.Send(isCatchFlag);
            CatchClientRpc(isCatchFlag);
        }

        //クライアントにキャッチしたことを伝える
        [ClientRpc]
        void CatchClientRpc(bool isCatchFlag)
        {
            if (IsOwner)
                return;

            inputSender.Catch.Send(isCatchFlag);
        }

        private void Start()
        {
            //初期化
            startScale = CatchEffectObject.localScale;

            inputSender.Catch.sender.Subscribe(x =>
            {
                if (x)
                {
                    //ボタン押したとき
                    CatchEffectObject
                        .DOScale(startScale + property.Catch.CatchEffectMaxSizeOffset,
                            property.Catch.CatchEffectAppearanceTime);

                    //サウンド再生
                    pool.Rent(catchCue).PlayAndReturnWhenStopped();
                }
                else
                {
                    //ボタン離した場合
                    CatchEffectObject.DOScale(startScale, property.Catch.CatchEffectDisappearingTime);

                    //サウンド再生
                    pool.Rent(catchCancelCue).PlayAndReturnWhenStopped();
                }

                if(IsSpawned && IsOwner)
                    CatchServerRpc(x);
            }).AddTo(this);

            playerProperty.HitDashAttack.Subscribe(x =>
            {
                //入力をリセット＆掴む処理無効化
                inputSender.Catch.Send(false);
                inputSender.Catch.isPause = true;

                //指定時間後に掴み無効かを解除する
                Observable.Timer(TimeSpan.FromSeconds(x.playerProperty.characterProperty.Attack.CatchInvalidTime)).Subscribe(_ =>
                {
                    inputSender.Catch.isPause = false;
                }).AddTo(this);
            }).AddTo(this);
        }
    }
}
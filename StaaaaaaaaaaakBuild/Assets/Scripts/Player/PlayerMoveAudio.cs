using System;
using DG.Tweening;
using StackBuild.Audio;
using UniRx;
using UnityEngine;

namespace StackBuild
{
    public class PlayerMoveAudio : MonoBehaviour
    {
        [SerializeField] private AudioCue cue;
        [SerializeField] private AudioSource source;
        private ModelSetup modelSetup;
        private InputSender inputSender => modelSetup.inputSender;
        private void Start()
        {
            //モデルセットアップを取得
            transform.parent.TryGetComponent(out modelSetup);

            //オーディオセット
            source.clip = cue.Clip;
            source.loop = cue.Loop;
            source.volume = 0f;
            source.Play();

            inputSender.Move.sender.Subscribe(x =>
            {
                if (x.sqrMagnitude == 0.0f)
                {
                    source.DOFade(0.0f, 0.5f).SetEase(Ease.OutCubic);
                }
                else
                {
                    source.DOFade(1.0f, 0.5f).SetEase(Ease.OutCirc);
                }
            }).AddTo(this);
        }
    }
}
using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
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

        private TweenerCore<float, float, FloatOptions> moveAudioFade;

        private void Start()
        {
            //モデルセットアップを取得
            if (transform.parent == null || !transform.parent.TryGetComponent(out modelSetup))
                return;

            //オーディオセット
            source.clip = cue.Clip;
            source.loop = cue.Loop;
            source.volume = 0f;
            source.Play();

            inputSender.Move.sender.Subscribe(x =>
            {
                if (x.sqrMagnitude == 0.0f)
                {
                    moveAudioFade.Kill();
                    moveAudioFade = source.DOFade(0.0f, 1.0f).SetEase(Ease.OutCubic);
                }
                else
                {
                    moveAudioFade.Kill();
                    moveAudioFade = source.DOFade(1.0f, 2.0f).SetEase(Ease.OutCirc);
                }
            }).AddTo(this);
        }
    }
}
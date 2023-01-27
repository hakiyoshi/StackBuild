using System;
using StackBuild.Audio;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace StackBuild
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioSourceWatching : MonoBehaviour
    {
        public bool isUse
        {
            get
            {
                return gameObject.activeSelf;
            }

            private set
            {
                gameObject.SetActive(value);
            }
        }

        private AudioSource Audio;
        public AudioSource audioSource
        {
            get
            {
                if (!isUse)
                    Debug.LogError("Called even though AudioSourceWatching's Active is false.");

                return Audio;
            }
        }

        private AudioSourcePool audioSourcePool = null;

        private void Awake()
        {
            TryGetComponent(out Audio);
        }

        public void StartOfUse(AudioCue cue, AudioSourcePool pool)
        {
            Audio.volume = 1.0f;
            Audio.clip = cue.Clip;
            Audio.loop = cue.Loop;
            audioSourcePool = pool;
            isUse = true;
        }

        public void PlayAndReturnWhenStopped(ulong delay = 0)
        {
            if (!isUse)
            {
                Debug.LogError("Called even though AudioSourceWatching's Active is false.");
                return;
            }

            Audio.Play(delay);
            this.UpdateAsObservable().Where(_ => !Audio.isPlaying).First().Subscribe(_ =>
            {
                ReturnAudio();
            }).AddTo(this);
        }

        public void ReturnAudio()
        {
            isUse = false;
            Audio.Stop();
            audioSourcePool.ReturnAudio(this);
        }
    }
}
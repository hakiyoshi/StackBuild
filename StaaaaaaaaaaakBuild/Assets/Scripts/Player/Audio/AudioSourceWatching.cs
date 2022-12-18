using System;
using StackBuild.Audio;
using UniRx;
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
                {
                    Debug.LogError("Called even though AudioSourceWatching's Active is false.");
                    return null;
                }

                return Audio;
            }
        }

        private bool isReturnStop = false;

        private void Awake()
        {
            TryGetComponent(out Audio);

            audioSource.ObserveEveryValueChanged(x => x.isPlaying).Where(x => !x && isReturnStop).Subscribe(_ =>
            {
                ReturnAudio();
            }).AddTo(this);
        }

        public void StartOfUse(AudioCue cue)
        {
            audioSource.clip = cue.Clip;
            audioSource.loop = cue.Loop;
            isUse = true;
        }

        public void PlayAndReturnWhenStopped(ulong delay = 0)
        {
            if (!isUse)
            {
                Debug.LogError("Called even though AudioSourceWatching's Active is false.");
                return;
            }

            audioSource.Play(delay);
            isReturnStop = true;
        }

        public void ReturnAudio()
        {
            isUse = false;
            isReturnStop = false;
            audioSource.Stop();
        }
    }
}
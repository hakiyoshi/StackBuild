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

        private void Awake()
        {
            TryGetComponent(out Audio);
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
            audioSource.ObserveEveryValueChanged(x => x.isPlaying).Where(x => !x).First().Subscribe(x =>
            {
                ReturnAudio();
            }).AddTo(this);
        }

        public void ReturnAudio()
        {
            isUse = false;
            audioSource.Stop();
        }
    }
}
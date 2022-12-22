using System;
using System.Collections.Generic;
using StackBuild.Audio;
using UnityEngine;

namespace StackBuild
{
    public class AudioSourcePool : MonoBehaviour
    {
        private Queue<AudioSourceWatching> audioPool = new ();
        [SerializeField] private GameObject AudioPrefab;
        [SerializeField] private Transform parentObject;
        [SerializeField] private int poolMaxSize = 5;

        private int audioCounter = 0;

        public AudioSourceWatching Rent(AudioCue cue)
        {
            if (!audioPool.TryDequeue(out AudioSourceWatching audio))
            {
                //新規作成前サイズチェック
                if (poolMaxSize >= 0 && audioCounter >= poolMaxSize)
                {
                    Debug.LogError("Number of objects in AudioSourcePool has reached the limit.");
                    return null;
                }

                //Audioがないまたは使われている場合
                audio = CreateAudioInstance();
            }

            audio.StartOfUse(cue, this);
            return audio;
        }

        AudioSourceWatching CreateAudioInstance()
        {
            audioCounter++;
            var obj = Instantiate(AudioPrefab, parentObject);
            return obj.GetComponent<AudioSourceWatching>();
        }

        public void ReturnAudio(AudioSourceWatching audio)
        {
            audioPool.Enqueue(audio);
        }
    }
}
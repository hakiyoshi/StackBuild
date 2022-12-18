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

        public AudioSourceWatching Rent(AudioCue cue)
        {
            if (!audioPool.TryDequeue(out AudioSourceWatching audio) || !audio.isUse)
            {
                if (poolMaxSize >= 0 && audioPool.Count >= poolMaxSize)
                {
                    Debug.LogError("Number of objects in AudioSourcePool has reached the limit.");
                    return null;
                }

                //Audioがないまたは使われている場合
                audio = CreateAudioInstance();
            }

            audioPool.Enqueue(audio);
            audio.StartOfUse(cue);
            return audio;
        }

        AudioSourceWatching CreateAudioInstance()
        {
            var obj = Instantiate(AudioPrefab, parentObject);
            return obj.GetComponent<AudioSourceWatching>();
        }
    }
}
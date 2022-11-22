using System;
using UnityEngine;
using UniRx;

namespace StackBuild.Audio
{
    [CreateAssetMenu(fileName = "New Effect Channel", menuName = "ScriptableObject/Audio/AudioEffectChannel")]
    public sealed class AudioEffectChannel : ScriptableObject
    {

        private readonly Subject<bool> onEffectRequest = new();

        public IObservable<bool> OnEffectRequest => onEffectRequest;

        public void RequestEffect(bool active)
        {
            onEffectRequest.OnNext(active);
        }

    }
}

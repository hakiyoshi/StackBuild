using UnityEngine;
using UniRx;

namespace StackBuild.Audio
{
    public sealed class ReverbEffectReceiver : MonoBehaviour
    {

        [SerializeField] private AudioEffectChannel channel;
        [SerializeField] private AudioReverbFilter filter;

        private void Awake()
        {
            // tweenあきらめた
            // たぶんばれない
            // UnityのAudio Mixer使えばいけそうではある
            channel.OnEffectRequest.Subscribe(active => filter.enabled = active).AddTo(this);
        }

    }
}

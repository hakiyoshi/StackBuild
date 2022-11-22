using UnityEngine;
using UniRx;

namespace StackBuild.Audio
{
    public sealed class AudioReceiver : MonoBehaviour
    {

        [SerializeField] private AudioChannel channel;
        [SerializeField] private AudioSource source;
        [SerializeField] private bool ignoreSameCue;
        [SerializeField] private bool allowLoop;

        private void Awake()
        {
            channel.OnRequest.Subscribe(cue =>
            {
                if (ignoreSameCue && source.clip == cue.Clip) return;
                source.clip = cue.Clip;
                source.loop = allowLoop && cue.Loop;
                source.Play();
            });
            channel.OnStopRequest.Subscribe(_ => { source.Stop(); });
        }

    }
}

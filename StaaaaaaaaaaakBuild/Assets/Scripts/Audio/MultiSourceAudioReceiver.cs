using UnityEngine;
using UniRx;

namespace StackBuild.Audio
{
    public sealed class MultiSourceAudioReceiver : MonoBehaviour
    {

        [SerializeField] private AudioChannel channel;
        [SerializeField] private AudioSource sourcePrefab;
        [SerializeField] private int sourceCount;

        private AudioSource[] sources;
        private int sourceIdx;

        private void Awake()
        {
            sources = new AudioSource[sourceCount];
            for (int i = 0; i < sourceCount; i++)
            {
                sources[i] = Instantiate(sourcePrefab, transform);
            }

            sourceIdx = 0;

            channel.OnRequest.Subscribe(cue =>
            {
                var source = sources[sourceIdx];
                sourceIdx = (sourceIdx + 1) % sources.Length;
                source.clip = cue.Clip;
                source.Play();
            }).AddTo(this);
            channel.OnStopRequest.Subscribe(_ =>
            {
                foreach (var source in sources)
                {
                    source.Stop();
                }
            }).AddTo(this);
        }

    }
}

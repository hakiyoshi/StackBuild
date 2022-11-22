using UnityEngine;
using DG.Tweening;
using UniRx;

namespace StackBuild.Audio
{
    public sealed class LowPassEffectReceiver : MonoBehaviour
    {

        private const float InitialCutoff = 22000;

        [SerializeField] private AudioEffectChannel channel;
        [SerializeField] private AudioLowPassFilter filter;
        [SerializeField] private float cutoff;
        [SerializeField] private float duration;

        private void Awake()
        {
            channel.OnEffectRequest.Subscribe(active =>
            {
                if (active) filter.enabled = true;
                DOTween.To(
                    () => filter.cutoffFrequency,
                    v => filter.cutoffFrequency = v,
                    active ? cutoff : InitialCutoff,
                    duration
                ).SetTarget(filter).SetUpdate(true).OnComplete(() =>
                {
                    if (!active) filter.enabled = false;
                });
            }).AddTo(this);
        }

    }
}

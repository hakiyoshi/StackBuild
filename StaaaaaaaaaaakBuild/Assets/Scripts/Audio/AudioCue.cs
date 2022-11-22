using UnityEngine;

namespace StackBuild.Audio
{
    [CreateAssetMenu(fileName = "New Audio Cue", menuName = "ScriptableObject/Audio/AudioCue")]
    public sealed class AudioCue : ScriptableObject
    {

        [field: SerializeField] public bool Loop { get; private set; }
        [field: SerializeField] public AudioClip Clip { get; private set; }

    }
}

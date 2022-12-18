using UnityEngine;

namespace StackBuild
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Cover Settings")]
    public class CoverSettings : ScriptableObject
    {
        [field: SerializeField] public float AppearanceTime { get; private set; } = 15.0f;
        [field: SerializeField] public float IntervalTime { get; private set; } = 5.0f;
    }
}
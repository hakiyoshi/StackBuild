using UnityEngine;

namespace StackBuild
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Canon Settings")]
    public class CanonSettings : ScriptableObject
    {
        [field: SerializeField] public float ShootDuration { get; private set; } = 0.07f;
        [field: SerializeField] public float ShootPower { get; private set; } = 10f;
        [field: SerializeField] public float ShootAngle { get; private set; } = 5f;
    }
}
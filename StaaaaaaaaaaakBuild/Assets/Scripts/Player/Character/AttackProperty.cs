using UnityEngine;

namespace StackBuild
{
    [CreateAssetMenu(menuName = "Attack")]
    public class AttackProperty : ScriptableObject
    {
        [field: SerializeField] public float CatchInvalidTime { get; private set; } = 0.0f;
        [field: SerializeField] public float StunTime { get; private set; } = 0.0f;
        [field: SerializeField] public float KnockbackPower { get; private set; } = 0.0f;
    }
}
using DG.Tweening;
using UnityEngine;

namespace StackBuild
{
    [CreateAssetMenu(menuName = "Game/CharacterProperty/Attack")]
    public class AttackProperty : ScriptableObject
    {
        [field: SerializeField] public float CatchInvalidTime { get; private set; } = 0.0f;
        [field: SerializeField] public float StunTime { get; private set; } = 0.0f;
        [field: SerializeField] public float KnockbackPower { get; private set; } = 0.0f;
        [field: SerializeField] public float KnockbackTime { get; private set; } = 0.5f;
        [field: SerializeField] public Ease KnockbackEase { get; private set; } = Ease.OutCubic;
    }
}
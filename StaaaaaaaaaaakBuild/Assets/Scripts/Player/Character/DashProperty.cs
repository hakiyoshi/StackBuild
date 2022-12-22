using DG.Tweening;
using UnityEngine;

namespace StackBuild
{
    [CreateAssetMenu(menuName = "Game/CharacterProperty/Dash")]
    public class DashProperty : ScriptableObject
    {
        [field: Header("Dash")]
        [field: SerializeField, Tooltip("最高速")] public float DashMaxSpeed { get; private set; } = 40.0f;
        [field: SerializeField, Tooltip("最高速までの時間(秒)")] public float DashAccelerationTime { get; private set; } = 0.2f;
        [field: SerializeField, Tooltip("速度0までの時間(秒)")] public float DashDeceleratingTime { get; private set; } = 0.6f;
        [field: SerializeField, Tooltip("加速時のイージング")] public Ease DashEaseOfAcceleration { get; private set; } = Ease.OutCirc;
        [field: SerializeField, Tooltip("減速時のイージング")] public Ease DashEaseOfDeceleration { get; private set; } = Ease.OutQuad;
        [field: SerializeField, Tooltip("秒")] public float DashCoolTime { get; private set; } = 1.0f;
    }
}
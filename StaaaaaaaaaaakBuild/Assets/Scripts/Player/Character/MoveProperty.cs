using UnityEngine;

namespace StackBuild
{
    [CreateAssetMenu(menuName = "Game/CharacterProperty/Move")]
    public class MoveProperty : ScriptableObject
    {
        [field: Header("Move")]
        [field: SerializeField, Tooltip("加速度")] public float Acceleration { get; private set; } = 1000.0f;
        [field: SerializeField, Tooltip("減速度")] public float Deceleration { get; private set; } = 0.99f;
        [field: SerializeField, Tooltip("最大速度")] public float MaxSpeed { get; private set; } = 20.0f;

        [field: Header("MoveAnimation")]
        [field: SerializeField, Tooltip("移動時の傾く角度(Euler)")] public float SlopeAngle{ get; private set; } = 10.0f;
        [field: SerializeField, Tooltip("傾く時間")] public float SlopeTime{ get; private set; } = 5.0f;
        [field: SerializeField, Tooltip("正面を向く時間")] public float LookForwardTime { get; private set; } = 5.0f;
    }
}
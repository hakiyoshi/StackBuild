using UnityEngine;

namespace StackBuild
{
    [CreateAssetMenu(menuName = "Game/CharacterProperty")]
    public class CharacterProperty : ScriptableObject
    {
        [field: Header("Model")]
        [field: SerializeField] public GameObject CharacterModelPrefab { get; private set; }
        [field: SerializeField] public Material EffectMaterial1 { get; private set; }
        [field: SerializeField] public Material EffectMaterial2 { get; private set; }

        [field: Header("Move")]
        [field: SerializeField, Tooltip("加速度")] public float Acceleration { get; private set; } = 1000.0f;
        [field: SerializeField, Tooltip("減速度")] public float Deceleration { get; private set; } = 0.99f;
        [field: SerializeField, Tooltip("最大速度")] public float MaxSpeed { get; private set; } = 20.0f;

        [field: Header("MoveAnimation")]
        [field: SerializeField, Tooltip("移動時の傾く角度(Euler)")] public float SlopeAngle{ get; private set; } = 10.0f;
        [field: SerializeField, Tooltip("傾く時間")] public float SlopeTime{ get; private set; } = 5.0f;
        [field: SerializeField, Tooltip("正面を向く時間")] public float LookForwardTime{ get; private set; } = 5.0f;

        [field: Header("Catch")]
        [field: SerializeField, Tooltip("掴む強さ")] public float CatchupPower { get; private set; } = 400.0f;
        [field: SerializeField, Tooltip("プレイヤー座標から掴む座標の差")] public Vector3 CatchupOffsetPosition { get; private set; } = new Vector3(0.0f, -5.0f, 0.0f);

        [field: Header("CatchAnimation")]
        [field: SerializeField, Tooltip("掴むエフェクトが出る時間")] public float CatchEffectAppearanceTime { get; private set; } = 0.2f;
        [field: SerializeField, Tooltip("掴むエフェクトが消える時間")] public float CatchEffectDisappearingTime { get; private set; } = 0.2f;
        [field: SerializeField, Tooltip("基準サイズから見たエフェクトの最大サイズの差")] public Vector3 CatchEffectMaxSizeOffset { get; private set; } = new Vector3(3.5f, 2.8f, 3.5f);
    }
}
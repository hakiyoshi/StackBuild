using DG.Tweening;
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
        [field: SerializeField, Tooltip("正面を向く時間")] public float LookForwardTime { get; private set; } = 5.0f;

        [field: Header("Dash")]
        [field: SerializeField, Tooltip("最高速")] public float DashMaxSpeed { get; private set; } = 0.1f;
        [field: SerializeField, Tooltip("最高速までの時間(秒)")] public float DashAccelerationTime { get; private set; } = 0.2f;
        [field: SerializeField, Tooltip("速度0までの時間(秒)")] public float DashDeceleratingTime { get; private set; } = 0.6f;
        [field: SerializeField, Tooltip("加速時のイージング")] public Ease DashEaseOfAcceleration { get; private set; } = Ease.OutCirc;
        [field: SerializeField, Tooltip("減速時のイージング")] public Ease DashEaseOfDeceleration { get; private set; } = Ease.OutQuad;
        [field: SerializeField, Tooltip("秒")] public float DashCoolTime { get; private set; } = 1.0f;

        [field: Header("DashEffect")]
        [field: SerializeField] public GameObject DashEffectPrefab;
        [field: SerializeField, Tooltip("エフェクト最大サイズ")] public Vector3 DashEffectMaxScale { get; private set; } = new Vector3(5.5f, 5.5f, 5.5f);
        [field: SerializeField, Tooltip("エフェクト最小サイズ")] public Vector3 DashEffectMinScale { get; private set; } = new Vector3(0f, 3.0f, 0f);
        [field: SerializeField, Tooltip("出現時間")] public float DashEffectAppearanceTime { get; private set; } = 0.05f;
        [field: SerializeField, Tooltip("消滅時間(DashDeceleratingTimeと同じがおすすめ)")] public float DashEffectExitTime { get; private set; } = 0.6f;

        [field: Header("Catch")]
        [field: SerializeField, Tooltip("掴む強さ")] public float CatchupPower { get; private set; } = 400.0f;
        [field: SerializeField, Tooltip("プレイヤー座標から掴む座標の差")] public Vector3 CatchupOffsetPosition { get; private set; } = new Vector3(0.0f, -5.0f, 0.0f);

        [field: Header("CatchAnimation")]
        [field: SerializeField, Tooltip("掴むエフェクトが出る時間")] public float CatchEffectAppearanceTime { get; private set; } = 0.2f;
        [field: SerializeField, Tooltip("掴むエフェクトが消える時間")] public float CatchEffectDisappearingTime { get; private set; } = 0.2f;
        [field: SerializeField, Tooltip("基準サイズから見たエフェクトの最大サイズの差")] public Vector3 CatchEffectMaxSizeOffset { get; private set; } = new Vector3(3.5f, 2.8f, 3.5f);
    }
}
using UnityEngine;

namespace StackBuild
{
    [CreateAssetMenu(menuName = "Game/CharacterProperty/Catch")]
    public class CatchProperty : ScriptableObject
    {
        [field: Header("Material")]
        [field: SerializeField] public Material EffectMaterial1 { get; private set; }
        [field: SerializeField] public Material EffectMaterial2 { get; private set; }

        [field: Header("Catch")]
        [field: SerializeField, Tooltip("掴む強さ")] public float CatchupPower { get; private set; } = 400.0f;
        [field: SerializeField, Tooltip("プレイヤー座標から掴む座標の差")] public Vector3 CatchupOffsetPosition { get; private set; } = new Vector3(0.0f, -5.0f, 0.0f);

        [field: Header("CatchAnimation")]
        [field: SerializeField, Tooltip("掴むエフェクトが出る時間")] public float CatchEffectAppearanceTime { get; private set; } = 0.2f;
        [field: SerializeField, Tooltip("掴むエフェクトが消える時間")] public float CatchEffectDisappearingTime { get; private set; } = 0.2f;
        [field: SerializeField, Tooltip("基準サイズから見たエフェクトの最大サイズの差")] public Vector3 CatchEffectMaxSizeOffset { get; private set; } = new Vector3(3.5f, 2.8f, 3.5f);

    }
}
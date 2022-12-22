using UnityEngine;

namespace StackBuild
{
    [CreateAssetMenu(menuName = "Game/CharacterProperty/Model")]
    public class ModelProperty : ScriptableObject
    {
        [field: SerializeField] public GameObject CharacterModelPrefab { get; private set; }
        [field: SerializeField] public float SphereColliderRadius { get; private set; } = 3.5f;
        [field: SerializeField] public GameObject HitEffect { get; private set; }

        [field: Header("DashEffect")]
        [field: SerializeField] public GameObject DashEffectPrefab;
        [field: SerializeField, Tooltip("エフェクト最大サイズ")] public Vector3 DashEffectMaxScale { get; private set; } = new Vector3(4.0f, 4.0f, 4.0f);
        [field: SerializeField, Tooltip("エフェクト最小サイズ")] public Vector3 DashEffectMinScale { get; private set; } = new Vector3(0f, 3.0f, 0f);
        [field: SerializeField, Tooltip("出現時間")] public float DashEffectAppearanceTime { get; private set; } = 0.05f;
        [field: SerializeField, Tooltip("消滅時間(DashDeceleratingTimeと同じがおすすめ)")] public float DashEffectExitTime { get; private set; } = 0.6f;
    }
}
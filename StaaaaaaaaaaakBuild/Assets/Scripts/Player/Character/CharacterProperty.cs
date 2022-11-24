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
        [field: SerializeField] public float Acceleration { get; private set; } = 1000.0f;
        [field: SerializeField] public float Deceleration { get; private set; } = 0.99f;
        [field: SerializeField] public float MaxSpeed { get; private set; } = 20.0f;

        [field: Header("Catch")]
        [field: SerializeField] public float CatchupPower { get; private set; } = 400.0f;
        [field: SerializeField] public Vector3 CatchupOffsetPosition { get; private set; } = new Vector3(0.0f, -5.0f, 0.0f);
        [field: SerializeField] public float CatchEffectAppearanceTime { get; private set; } = 0.2f;
        [field: SerializeField] public float CatchEffectDisappearingTime { get; private set; } = 0.2f;

        [field: SerializeField] public Vector3 CatchEffectMaxSizeOffset { get; private set; } = new Vector3(3.5f, 2.8f, 3.5f);
    }
}
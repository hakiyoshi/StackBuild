using UnityEngine;

namespace StackBuild
{
    [CreateAssetMenu(menuName = "Game/PlayerProperty")]
    public class PlayerProperty : ScriptableObject
    {
        [field: Header("Move")]
        [field: SerializeField] public float Acceleration { get; private set; } = 1000.0f;
        [field: SerializeField] public float Deceleration { get; private set; } = 0.99f;
        [field: SerializeField] public float MaxSpeed { get; private set; } = 20.0f;

        [field: Header("Catch")]
        [field: SerializeField] public float CatchupPower { get; private set; } = 400.0f;
        [field: SerializeField] public Vector3 CatchupOffsetPosition { get; private set; } = new Vector3(0.0f, -5.0f, 0.0f);
    }
}
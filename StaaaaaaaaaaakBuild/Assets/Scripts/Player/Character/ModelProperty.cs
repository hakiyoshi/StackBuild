using UnityEngine;

namespace StackBuild
{
    [CreateAssetMenu(menuName = "Game/CharacterProperty/Model")]
    public class ModelProperty : ScriptableObject
    {
        [field: SerializeField] public GameObject CharacterModelPrefab { get; private set; }
        [field: SerializeField] public float SphereColliderRadius { get; private set; } = 3.5f;
    }
}
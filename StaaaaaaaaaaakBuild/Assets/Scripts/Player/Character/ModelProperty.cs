using UnityEngine;

namespace StackBuild
{
    [CreateAssetMenu(menuName = "Game/CharacterProperty/Model")]
    public class ModelProperty : ScriptableObject
    {
        [field: SerializeField] public GameObject CharacterModelPrefab { get; private set; }

    }
}
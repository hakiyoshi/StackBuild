using DG.Tweening;
using UnityEngine;

namespace StackBuild
{
    [CreateAssetMenu(menuName = "Game/CharacterProperty/CharacterProperty")]
    public class CharacterProperty : ScriptableObject
    {
        [field: SerializeField] public GameObject CharacterModelPrefab { get; private set; }
        [field: SerializeField] public MoveProperty Move { get; private set; }
        [field: SerializeField] public DashProperty Dash { get; private set; }
        [field: SerializeField] public CatchProperty Catch { get; private set; }
    }
}
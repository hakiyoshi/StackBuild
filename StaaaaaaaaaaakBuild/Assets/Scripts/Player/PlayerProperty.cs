using UniRx;
using UnityEngine;

namespace StackBuild
{
    [CreateAssetMenu(menuName = "Game/PlayerProperty")]
    public class PlayerProperty : ScriptableObject
    {
        [field: SerializeField] public CharacterProperty characterProperty { get; private set; }

        public Subject<AttackProperty> DashHitAction { get; private set; } = new Subject<AttackProperty>();


        public void Initialize(CharacterProperty character)
        {
            characterProperty = character;
        }
    }
}
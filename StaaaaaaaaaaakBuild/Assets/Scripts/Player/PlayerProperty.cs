using UniRx;
using UnityEngine;

namespace StackBuild
{
    [CreateAssetMenu(menuName = "Game/PlayerProperty")]
    public class PlayerProperty : ScriptableObject
    {
        [field: SerializeField] public CharacterProperty characterProperty { get; private set; }

        public GameObject PlayerObject { get; private set; }
        public Subject<PlayerProperty> DashHitAction { get; private set; } = new Subject<PlayerProperty>();


        public void Initialize(CharacterProperty character)
        {
            characterProperty = character;
        }
    }
}
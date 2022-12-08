using UniRx;
using UnityEngine;

namespace StackBuild
{
    [CreateAssetMenu(menuName = "Game/PlayerProperty")]
    public class PlayerProperty : ScriptableObject
    {
        [field: SerializeField] public CharacterProperty characterProperty { get; private set; }

        public GameObject PlayerObject = null;
        public Subject<PlayerProperty> HitDashAttack { get; private set; } = new Subject<PlayerProperty>();


        public void Initialize(CharacterProperty character)
        {
            characterProperty = character;
        }
    }
}
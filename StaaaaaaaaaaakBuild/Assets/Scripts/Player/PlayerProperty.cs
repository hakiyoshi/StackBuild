using UniRx;
using UnityEngine;

namespace StackBuild
{
    [CreateAssetMenu(menuName = "Game/PlayerProperty")]
    public class PlayerProperty : ScriptableObject
    {
        [field: SerializeField] public CharacterProperty characterProperty { get; private set; }

        [HideInInspector] public GameObject PlayerObject = null;

        public struct DashAttackInfo
        {
            public PlayerProperty playerProperty;

            public DashAttackInfo(PlayerProperty playerProperty)
            {
                this.playerProperty = playerProperty;
            }
        }
        public Subject<DashAttackInfo> HitDashAttack { get; private set; } = new Subject<DashAttackInfo>();


        public void Initialize(CharacterProperty character)
        {
            characterProperty = character;
        }
    }
}
using UnityEngine;

namespace StackBuild
{
    [CreateAssetMenu(menuName = "Game/PlayerManagerProperty")]
    public class PlayerManagerProperty : ScriptableObject
    {
        [HideInInspector] public PlayerManager playerManager;
    }
}
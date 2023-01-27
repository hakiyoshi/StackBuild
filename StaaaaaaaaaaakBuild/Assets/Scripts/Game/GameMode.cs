using UnityEngine;

namespace StackBuild.Game
{
    [CreateAssetMenu(fileName = "New Game Mode", menuName = "ScriptableObject/GameMode", order = 0)]
    public class GameMode : ScriptableObject
    {
        public static GameMode Current { get; set; }
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public bool IsOnline { get; private set; }
        [field: SerializeField] public bool IsPractice { get; private set; }
        [field: SerializeField] public PlayerProperty[] PlayersToSelectCharacter { get; private set; }

    }
}
using System;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(menuName = "Game/GameProperty")]
    public class GameProperty : ScriptableObject
    {
        public const int MAXPLAYER = 2;
        public int NumPlayer = MAXPLAYER;

        public enum GameState
        {
            Intro,
            Game,
            Result,
            Pose,
        }
        public GameState State = GameState.Intro;

        public void Reset()
        {
            NumPlayer = MAXPLAYER;
            State = GameState.Intro;
        }
    }
}
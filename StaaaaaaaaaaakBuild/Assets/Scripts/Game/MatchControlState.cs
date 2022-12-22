using UniRx;
using UnityEngine;

namespace StackBuild.Game
{
    [CreateAssetMenu(menuName = "ScriptableObject/MatchControl/MatchControlState")]
    public class MatchControlState : ScriptableObject
    {
        public IReadOnlyReactiveProperty<MatchState> State => state;
        private ReactiveProperty<MatchState> state = new ReactiveProperty<MatchState>();

        public void SendState(MatchState matchState)
        {
            state.Value = matchState;
        }
    }
}
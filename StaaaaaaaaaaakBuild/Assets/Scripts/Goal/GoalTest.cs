using UnityEngine;

namespace StackBuild
{
    public class GoalTest : MonoBehaviour
    {
        [SerializeField] private GoalCore goal;
        [SerializeField] private BuildingData data;

        private void Start()
        {
            if (goal == null || data == null) return;

            goal.Enqueue(data);
        }
    }
}
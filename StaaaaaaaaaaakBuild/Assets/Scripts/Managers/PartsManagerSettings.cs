using System.Collections.Generic;
using UnityEngine;

namespace StackBuild
{
    [System.Serializable]
    public class PartsSpawnRule
    {
        public int threshould;
        public int count;
        public float minSeconds;
        public float maxSeconds;
    }

    [CreateAssetMenu(menuName = "Scriptable Objects/Parts Manager Settings")]
    public class PartsManagerSettings : ScriptableObject
    {
        [field: SerializeField] public List<PartsSpawnRule> SpawnRuleList { get; private set; } = new();
    }
}
using System.Collections.Generic;
using UnityEngine;

namespace StackBuild
{
    public enum MaterialId
    {
        Metal,
        Plastic,
        Wood,
    }

    [System.Serializable]
    public class FloorData
    {
        public string name;
        public int score;
        public List<BuildCubeId> buildingIds;
        public SDictionary<MaterialId, int> needMaterials;
    }

    [CreateAssetMenu(menuName = "Scriptable Objects/Goal Settings")]
    public class GoalSettings : ScriptableObject
    {
        [field: SerializeField] public bool IsLocalPlayTest { get; private set; } = false;
        [field: SerializeField] public List<FloorData> FloorDataList { get; private set; } = new();
    }
}
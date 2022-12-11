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
    public class BuildingData
    {
        public string name;
        public Material material;
        public Mesh mesh;
        public int count;
        public int score;
        public SDictionary<MaterialId, int> needMaterials;
    }

    [CreateAssetMenu(menuName = "Scriptable Objects/Goal Settings")]
    public class GoalSettings : ScriptableObject
    {
        [field: SerializeField] public float WidthSize { get; private set; } = 12f;
        [field: SerializeField] public float HeightSize { get; private set; } = 3f;
        [field: SerializeField] public int Column { get; private set; } = 3;
        [field: SerializeField] public int Row { get; private set; } = 3;
        [field: SerializeField] public float MaxHeight { get; private set; } = 10f;
        [field: SerializeField] public float PartsFallTime { get; private set; } = 0.8f;
        [field: SerializeField] public float BaseFallTime { get; private set; } = 1.0f;
        [field: SerializeField] public float LoopTime { get; private set; } = 0.05f;
        [field: SerializeField] public List<BuildingData> BuildingDataList { get; private set; } = new();
    }
}
using UnityEngine;

namespace StackBuild
{
    public enum BuildCubeId
    {
        MetalCenter,
        MetalMid,
        MetalPillar,

        PlasticCenter,
        PlasticMid,
        PlasticPillar,

        WoodCenter,
        WoodMid,
        WoodPillar,
    }

    [System.Serializable]
    public class BuildCubeData
    {
        public Material material;
        public Mesh mesh;
    }

    [CreateAssetMenu(menuName = "Scriptable Objects/Building Settings")]
    public class BuildSettings : ScriptableObject
    {
        [field: SerializeField] public float WidthSize { get; private set; } = 12f;
        [field: SerializeField] public float HeightSize { get; private set; } = 3f;
        [field: SerializeField] public int Column { get; private set; } = 3;
        [field: SerializeField] public int Row { get; private set; } = 3;
        [field: SerializeField] public float MaxHeight { get; private set; } = 10f;
        [field: SerializeField] public float PartsFallTime { get; private set; } = 0.8f;
        [field: SerializeField] public float BaseFallTime { get; private set; } = 1.0f;
        [field: SerializeField] public float LoopTime { get; private set; } = 0.05f;
        [field: SerializeField] public float FinishedUpTime { get; private set; } = 1.0f;
        [field: SerializeField] public GameObject StackPrefab { get; private set; } = null;
        [field: SerializeField] public SDictionary<BuildCubeId, BuildCubeData> BuildingDataDictionary { get; private set; } = new();
    }
}
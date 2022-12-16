using UnityEngine;

namespace StackBuild
{
    public enum BuildingId
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
    public class BuildingData
    {
        public Material material;
        public Mesh mesh;
    }

    [CreateAssetMenu(menuName = "Scriptable Objects/Building Settings")]
    public class BuildSettings : ScriptableObject
    {
        public SDictionary<BuildingId, BuildingData> BuildingDataDictionary = new();
    }
}
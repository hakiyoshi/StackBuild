using System;
using System.Collections.Generic;
using UnityEngine;

namespace StackBuild
{
    public enum PartsId
    {
        Default,

        SmallMetal,
        SmallPlastic,
        SmallWood,

        MediumMetal,
        MediumPlastic,
        MediumWood,

        LargeMetal,
        LargePlastic,
        LargeWood,
    }

    [Serializable]
    public class PartsData
    {
        public Material material;
        public Mesh mesh;
        public SDictionary<MaterialId, int> containsMaterials;
    }

    [CreateAssetMenu(menuName = "Scriptable Objects/Parts Settings")]
    public class PartsSettings : ScriptableObject
    {
        [field: SerializeField]
        public SDictionary<PartsId, PartsData> PartsDataDictionary { get; private set; } = new();
    }
}
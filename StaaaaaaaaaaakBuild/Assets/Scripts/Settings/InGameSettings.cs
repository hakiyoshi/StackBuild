using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StackBuild
{
    public enum PartsId
    {
        Default,

        SmallCube,

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

    public enum MaterialId
    {
        Metal,
        Plastic,
        Wood,
    }

    [Serializable]
    public class PartsData
    {
        public Material material;
        public Mesh mesh;
        public SDictionary<MaterialId, int> containsMaterials;
    }
    [Serializable]
    public class PartsSpawnRule
    {
        public int threshould;
        public int count;
        public float minSeconds;
        public float maxSeconds;
    }
    [Serializable]
    public class StackData
    {
        public string name;
        public Material material;
        public Mesh mesh;
        public int count;
        public int score;
        public SDictionary<MaterialId, int> needMaterials;
    }

    [CreateAssetMenu(menuName = "StackBuild/In-Game Settings")]
    public class InGameSettings : ScriptableObject
    {
        [Header("Parts")]
        public SDictionary<PartsId, PartsData> partsDataDictionary = new();
        public List<PartsSpawnRule> spawnRuleList = new();

        [Header("Building")]
        public List<StackData> stackDataList = new();
    }
}


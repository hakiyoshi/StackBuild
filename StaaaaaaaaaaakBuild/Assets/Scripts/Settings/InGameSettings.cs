using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StackBuild
{
    [Serializable]
    public class PartsMeshData
    {
        public string name;
        public Material material;
        public Mesh mesh;
    }
    [Serializable]
    public class PartsSpawnRule
    {
        public int threshould;
        public int count;
        public float minSeconds;
        public float maxSeconds;
    }

    [CreateAssetMenu(menuName = "StackBuild/In-Game Settings")]
    public class InGameSettings : ScriptableObject
    {
        [Header("Parts")]
        public List<PartsMeshData> partsMeshList = new();
        public List<PartsSpawnRule> spawnRuleList = new();
    }
}


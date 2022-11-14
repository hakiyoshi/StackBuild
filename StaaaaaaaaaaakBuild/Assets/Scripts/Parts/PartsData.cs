using System.Collections.Generic;
using UnityEngine;

namespace StackBuild
{
    [CreateAssetMenu(menuName = "StackBuild/Parts Data", fileName = "Parts Data")]
    public class PartsData : ScriptableObject
    {
        public List<Parts> prefabs = new List<Parts>();
    }
}
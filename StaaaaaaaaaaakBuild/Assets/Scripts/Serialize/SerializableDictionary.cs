using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Rendering;

namespace StackBuild
{
    [Serializable]
    public class SKeyValuePair<TKey, TValue>
    {
        public TKey Key;
        public TValue Value;
    }

    [Serializable]
    public class SDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField] private List<SKeyValuePair<TKey, TValue>> dictToList;

        public SDictionary()
        {
            dictToList = new();
        }

        public void OnBeforeSerialize()
        {
            this.Clear();
        }

        public void OnAfterDeserialize()
        {
            foreach (var data in dictToList)
            {
                this.Add(data.Key, data.Value);
            }
        }
    }
}

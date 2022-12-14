using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace StackBuild
{
    [Serializable]
    public class SKeyValuePair<TKey, TValue>
    {
        public TKey Key;
        public TValue Value;

        /*
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Key);
            serializer.SerializeValue(ref Value);
        }
        */
    }

    [Serializable]
    public class SDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField] private List<SKeyValuePair<TKey, TValue>> dictToList = new();

        public SDictionary()
        {
            OnAfterDeserialize();
        }

        public void OnAfterDeserialize()
        {
            this.Clear();

            foreach (var data in dictToList)
            {
                if (!this.TryAdd(data.Key, data.Value))
                {
                    Debug.LogAssertion($"Serializable Dictionary: 重複している項目があります。({data.Key})");
                }
            }
        }

        public void OnBeforeSerialize() {}
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

namespace StackBuild
{
    public class PartsManager : MonoBehaviour
    {
        [SerializeField] private PartsSettings partsSettings;
        [SerializeField] private PartsManagerSettings settings;
        [SerializeField] private CanonManager canonManager;

        private PartsId[] IDArray => partsSettings.PartsDataDictionary.Keys.ToArray();
        private Queue<PartsCore> queue = new();
        private PartsCore[] children;

        private void Start()
        {
            children = GetComponentsInChildren<PartsCore>();
            foreach (var parts in children)
            {
                queue.Enqueue(parts);
            }
            StartCoroutine(SpawnTimerCoroutine());
        }

        IEnumerator SpawnTimerCoroutine()
        {
            while (true)
            {
                if (!settings.isLocalPlayTest && (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsServer))
                {
                    yield return null;
                    continue;
                }

                var rule = settings.SpawnRuleList.Find(x => x.threshould > GetActiveCount());
                if (rule == null)
                {
                    yield return null;
                    continue;
                }

                int index = canonManager.GetRandomIndex();
                for (int i = 0; i < rule.count; i++)
                {
                    var parts = Rent(GetRandomPartsId());
                    canonManager.Enqueue(index, parts);
                }

                yield return new WaitForSeconds(Random.Range(rule.minSeconds, rule.maxSeconds));
            }
        }

        public PartsCore Rent(PartsId id = PartsId.Default)
        {
            if (queue.TryDequeue(out var parts))
            {
                parts.partsId.Value = id;
            }
            else
            {
                Debug.Log("PartsManager: 貸し出せるPartsはありません。");
            }
            return parts;
        }

        public void Return(PartsCore parts)
        {
            if (!parts.transform.IsChildOf(transform)) return;

            var net = parts.GetComponent<PartsNetworkSync>();
            if (!settings.isLocalPlayTest || (net.IsSpawned && NetworkManager.Singleton != null && !NetworkManager.Singleton.IsServer)) return;

            parts.isActive.Value = false;
            parts.partsId.Value = PartsId.Default;

            queue.Enqueue(parts);
        }

        public PartsId GetRandomPartsId()
        {
            return IDArray[Random.Range(1, IDArray.Length)];
        }

        public int GetActiveCount()
        {
            return children.Length - queue.Count;
        }

        public int GetCount()
        {
            return children.Length;
        }
    }
}
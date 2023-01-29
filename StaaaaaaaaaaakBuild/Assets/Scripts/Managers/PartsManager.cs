using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using StackBuild.Game;
using UniRx;
using UniRx.Triggers;
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
        [SerializeField] private MatchControl matchControl;

        private PartsId[] IDArray => partsSettings.PartsDataDictionary.Keys.ToArray();
        private Queue<PartsCore> queue = new();
        private PartsCore[] children;
        private Dictionary<int, Rigidbody> partsRigidbodyDictionary = new();
        private CancellationTokenSource cts;

        private void Start()
        {
            children = GetComponentsInChildren<PartsCore>();
            foreach (var parts in children)
            {
                queue.Enqueue(parts);
            }

            var rbs = GetComponentsInChildren<Rigidbody>();
            foreach (var rb in rbs)
            {
                partsRigidbodyDictionary[rb.gameObject.GetInstanceID()] = rb;
            }

            if (matchControl == null) return;
            matchControl.State.Subscribe(state =>
            {
                if (state == MatchState.Ingame)
                {
                    StartSpawnTimer();
                }
                else
                {
                    StopSpawnTimer();
                }
            }).AddTo(this);
        }

        private void StartSpawnTimer()
        {
            StopSpawnTimer();
            cts = new();
            cts.AddTo(this);

            SpawnTimerAsync(cts.Token).Forget();
        }

        private void StopSpawnTimer()
        {
            if (cts == null) return;

            cts.Cancel();
            cts.Dispose();
            cts = null;
        }

        private async UniTask SpawnTimerAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var rule = settings.SpawnRuleList.Find(x => x.threshould > GetActiveCount());
                if (rule == null || NetworkManager.Singleton != null && !NetworkManager.Singleton.IsServer)
                {
                    await UniTask.Yield(PlayerLoopTiming.Update, token);
                    continue;
                }

                int index = canonManager.GetRandomIndex();
                for (int i = 0; i < rule.count; i++)
                {
                    var parts = Rent(GetRandomPartsId());
                    canonManager.Enqueue(index, parts);
                }

                await UniTask.Delay(TimeSpan.FromSeconds(Random.Range(rule.minSeconds, rule.maxSeconds)), cancellationToken: token);
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
            if (net.IsSpawned && NetworkManager.Singleton != null && !NetworkManager.Singleton.IsServer) return;

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

        public Rigidbody GetPartsRigidbody(int instanceId)
        {
            return partsRigidbodyDictionary[instanceId];
        }
    }
}
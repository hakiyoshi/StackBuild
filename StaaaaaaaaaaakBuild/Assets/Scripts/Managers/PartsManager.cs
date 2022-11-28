using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using VContainer;
using Random = UnityEngine.Random;

namespace StackBuild
{
    public class PartsManager : MonoBehaviour
    {
        [SerializeField] private InGameSettings settings;
        [SerializeField] private PartsCore prefab;
        [SerializeField] private CanonManager canonManager;
        public PartsPool pool { get; private set; }

        public int count => pool.Count;

        private void Start()
        {
            pool = new PartsPool(prefab, transform);

            SpawnTimerAsync(this.GetCancellationTokenOnDestroy()).Forget();
        }

        private PartsCore RandomMeshSpawn()
        {
            return Spawn(Random.Range(0, settings.partsMeshList.Count));
        }

        private PartsCore Spawn(int index)
        {
            var core = pool.Rent();
            if (core.gameObject.TryGetComponent(out PartsMesh partsMesh))
            {
                partsMesh.SetPartsData(index);
            }
            return core;
        }

        private async UniTaskVoid SpawnTimerAsync(CancellationToken token = default)
        {
            while (!token.IsCancellationRequested)
            {
                var spawnRule = settings.spawnRuleList.Find(x => x.threshould < pool.Count);

                for (int i = 0; i < spawnRule.count; i++)
                {
                    canonManager.RandomEnqueue(RandomMeshSpawn());
                }

                await UniTask.Delay(TimeSpan.FromSeconds(Random.Range(spawnRule.minSeconds, spawnRule.maxSeconds)), cancellationToken: token);
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
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

        private PartsId[] idArray;
        public PartsPool pool { get; private set; }

        public int count => pool.Count;

        private void Start()
        {
            pool = new PartsPool(prefab, transform);

            idArray = settings.partsDataDictionary.Keys.ToArray();

            SpawnTimerAsync(this.GetCancellationTokenOnDestroy()).Forget();
        }

        private PartsCore RandomMeshSpawn()
        {
            return Spawn( idArray[Random.Range(0, idArray.Count())] );
        }

        private PartsCore Spawn(PartsId id)
        {
            var core = pool.Rent();
            core.SetPartsID(id);
            return core;
        }

        private async UniTaskVoid SpawnTimerAsync(CancellationToken token = default)
        {
            while (!token.IsCancellationRequested)
            {
                var spawnRule = settings.spawnRuleList.Find(x => x.threshould <= pool.Count);

                if (spawnRule == null)
                {
                    await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken: token);
                    continue;
                }

                for (int i = 0; i < spawnRule.count; i++)
                {
                    canonManager.RandomEnqueue(RandomMeshSpawn());
                }

                await UniTask.Delay(TimeSpan.FromSeconds(Random.Range(spawnRule.minSeconds, spawnRule.maxSeconds)), cancellationToken: token);
            }
        }
    }
}
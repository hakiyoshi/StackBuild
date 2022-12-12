using System;
using System.Collections.Generic;
using System.Threading;
using System.Transactions.Configuration;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace StackBuild
{
    public class GoalCore : MonoBehaviour
    {
        [SerializeField] private GoalSettings settings;
        [SerializeField] private PartsManager partsManager;
        [SerializeField] private GameObject collisionObject;
        [SerializeField] private GameObject stackPrefab;
        [SerializeField] private Transform stackPos;

        private float WidthSize => settings.WidthSize;
        private float HeightSize => settings.HeightSize;
        private int Column => settings.Column;
        private int Row => settings.Row;
        private float MaxHeight => settings.MaxHeight;
        private float PartsFallTime => settings.PartsFallTime;
        private float BaseFallTime => settings.BaseFallTime;
        private float LoopTime => settings.LoopTime;

        private Dictionary<MaterialId, int> partsCount = new();
        private Queue<BuildingData> stackQueue = new();
        private GameObject buildingBase;
        private float height = 0;
        private float totalHeight = 0;
        private int floorPartsCount = 0;

        private void Start()
        {
            buildingBase = Instantiate(stackPrefab, stackPos);

            foreach (var id in (MaterialId[])Enum.GetValues(typeof(MaterialId)))
            {
                partsCount[id] = 0;
            }

            collisionObject.OnTriggerEnterAsObservable()
                .Where(x => x.gameObject.CompareTag("Parts"))
                .Select(x => x.gameObject.GetComponent<PartsCore>())
                .Where(x => x.PartsID.Value != PartsId.Default)
                .Subscribe(parts =>
                {
                    foreach (var buildMaterial in parts.GetPartsData().containsMaterials)
                    {
                        partsCount[buildMaterial.Key] += buildMaterial.Value;
                    }
                    partsManager.pool.Return(parts);

                    foreach (var data in settings.BuildingDataList)
                    {
                        bool isFailed = false;
                        foreach (var need in data.needMaterials)
                        {
                            if (partsCount[need.Key] < need.Value)
                            {
                                isFailed = true;
                                break;
                            }
                        }
                        if (isFailed) continue;

                        foreach (var need in data.needMaterials)
                        {
                            partsCount[need.Key] -= need.Value;
                        }
                        stackQueue.Enqueue(data);
                    }
                });

            StackLoopAsync(this.GetCancellationTokenOnDestroy()).Forget();
        }

        public void Enqueue(BuildingData data)
        {
            stackQueue.Enqueue(data);
        }

        public void Finished()
        {
            var pos = transform.position;
            pos.y = 0;
            transform.position = pos;
        }

        private async UniTask StackLoopAsync(CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();

            while (!token.IsCancellationRequested)
            {
                while (stackQueue.TryDequeue(out var data))
                {
                    for (int i = 0; i < data.count; i++)
                    {
                        StackAsync(data, token).Forget();
                        await BaseDownAsync(token);
                        await UniTask.Delay(TimeSpan.FromSeconds(LoopTime), cancellationToken: token);
                    }
                }
                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken: token);
            }
        }

        private async UniTask StackAsync(BuildingData data, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();

            var obj = Instantiate(stackPrefab, buildingBase.transform);
            if (!obj.TryGetComponent(out MeshRenderer meshRenderer)) return;

            obj.transform.localPosition = new Vector3(
                x: (Row - floorPartsCount / Column) * (WidthSize / Row) + (WidthSize / Row) - (WidthSize / 2),
                y: totalHeight + height + 20,
                z: (floorPartsCount % Column) * (WidthSize / Column) + (WidthSize / Column) - (WidthSize / 2));
            obj.transform.localScale = new Vector3(WidthSize / Row, HeightSize, WidthSize / Column);

            meshRenderer.sharedMaterial = data.material;
            meshRenderer.enabled = true;

            floorPartsCount++;

            if (floorPartsCount >= Row * Column)
            {
                height += HeightSize;
                floorPartsCount = 0;
            }

            await obj.transform.DOLocalMoveY(-20, PartsFallTime)
                .SetEase(Ease.InCubic)
                .SetRelative(true)
                .WithCancellation(token);
        }

        private async UniTask BaseDownAsync(CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();

            if (height < MaxHeight + 1) return;

            totalHeight += MaxHeight;
            height -= MaxHeight;

            await buildingBase.transform.DOLocalMoveY(-MaxHeight, BaseFallTime)
                .SetEase(Ease.OutBack)
                .SetDelay(PartsFallTime)
                .SetRelative(true)
                .WithCancellation(token);
        }
    }
}
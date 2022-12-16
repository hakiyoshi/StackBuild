using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

namespace StackBuild
{
    public class BuildingCore : MonoBehaviour
    {
        [SerializeField] private BuildSettings settings;
        [SerializeField] private Transform stackPos;

        private Queue<BuildCubeId> stackQueue = new();
        private GameObject buildingBase;
        private float height = 0;
        private float totalHeight = 0;
        private int floorPartsCount = 0;

        private float WidthSize => settings.WidthSize;
        private float HeightSize => settings.HeightSize;
        private int Column => settings.Column;
        private int Row => settings.Row;
        private float MaxHeight => settings.MaxHeight;
        private float PartsFallTime => settings.PartsFallTime;
        private float BaseFallTime => settings.BaseFallTime;
        private float LoopTime => settings.LoopTime;
        public float TotalHeight => totalHeight;

        public void Enqueue(BuildCubeId id)
        {
            stackQueue.Enqueue(id);
        }

        private void Start()
        {
            buildingBase = Instantiate(settings.StackPrefab, stackPos);

            StackLoopAsync(this.GetCancellationTokenOnDestroy()).Forget();
        }

        private async UniTask StackLoopAsync(CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();

            while (!token.IsCancellationRequested)
            {
                while (stackQueue.TryDequeue(out var buildingId))
                {
                    StackAsync(settings.BuildingDataDictionary[buildingId], token).Forget();
                    await BaseDownAsync(token);
                    await UniTask.Delay(TimeSpan.FromSeconds(LoopTime), cancellationToken: token);
                }
                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken: token);
            }
        }

        private async UniTask StackAsync(BuildCubeData data, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();

            var obj = Instantiate(settings.StackPrefab, buildingBase.transform);

            obj.transform.localPosition = new Vector3(
                x: (Row - floorPartsCount / Column) * (WidthSize / Row) + (WidthSize / Row) - (WidthSize / 2),
                y: totalHeight + height + 20,
                z: (floorPartsCount % Column) * (WidthSize / Column) + (WidthSize / Column) - (WidthSize / 2));
            obj.transform.localRotation = Quaternion.Euler(0f, Random.Range(0, 3) * 90f, 0f);
            obj.transform.localScale = new Vector3(WidthSize / Row, HeightSize, WidthSize / Column);

            if (obj.TryGetComponent(out MeshFilter meshFilter))
            {
                meshFilter.sharedMesh = data.mesh;
            }

            if (obj.TryGetComponent(out MeshRenderer meshRenderer))
            {
                meshRenderer.sharedMaterial = data.material;
                meshRenderer.enabled = true;
            }

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

            if (height <= MaxHeight) return;

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
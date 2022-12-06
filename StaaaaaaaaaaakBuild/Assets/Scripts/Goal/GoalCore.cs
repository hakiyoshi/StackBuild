using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Palmmedia.ReportGenerator.Core;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace StackBuild.Goal
{
    public class GoalCore : MonoBehaviour
    {
        [SerializeField] private InGameSettings settings;
        [SerializeField] private GameObject collisionObject;
        [SerializeField] private GameObject stackPrefab;
        [SerializeField] private Transform stackPos;
        [SerializeField] private float widthSize = 10f;
        [SerializeField] private float heightSize = 1f;
        [SerializeField] private int column = 1;
        [SerializeField] private int row = 1;
        [SerializeField] private int maxHeight;

        private Dictionary<MaterialId, int> partsCount = new();
        private Queue<StackData> stackQueue = new();
        private GameObject buildingBase;
        private int height = 0;
        private int totalHeight = 0;
        private int floorPartsCount = 0;

        private void Start()
        {
            buildingBase = Instantiate(stackPrefab, transform);
            buildingBase.transform.position += Vector3.down * -0.5f;

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

                    foreach (var data in settings.stackDataList)
                    {
                        foreach (var need in data.needMaterials)
                        {
                            if (partsCount[need.Key] < need.Value) continue;

                            stackQueue.Enqueue(data);
                        }
                    }
                });

            StackLoopAsync(this.GetCancellationTokenOnDestroy()).Forget();
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
                        await StackAsync(data, token);
                        await UniTask.Delay(TimeSpan.FromSeconds(0.05f), cancellationToken: token);
                        await BaseDownAsync(token);
                    }
                }
                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken: token);
            }
        }

        private async UniTask StackAsync(StackData data, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();

            var obj = Instantiate(stackPrefab, buildingBase.transform);
            if (!obj.TryGetComponent(out MeshRenderer renderer)) return;

            obj.transform.localPosition = new Vector3(
                x: (row - 1 - floorPartsCount / column) * (widthSize / row) + (widthSize / row) - (widthSize / 2),
                y: height + 20,
                z: (floorPartsCount % column) * (widthSize / column) + (widthSize / column) - (widthSize / 2));
            obj.transform.localScale = new Vector3(widthSize / row, heightSize, widthSize / column);

            renderer.sharedMaterial = data.material;
            renderer.enabled = true;

            await obj.transform.DOLocalMoveY(-20, 0.8f)
                .SetEase(Ease.InCubic)
                .SetRelative(true)
                .WithCancellation(token);

            floorPartsCount++;

            if (floorPartsCount > row * column)
            {
                height++;
                floorPartsCount = 0;
            }
        }

        private async UniTask BaseDownAsync(CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();

            if (height < maxHeight) return;

            totalHeight += height;
            height = 0;

            await transform.DOLocalMoveY(-maxHeight, 1.0f)
                .SetEase(Ease.OutBack)
                .SetDelay(0.8f)
                .SetRelative(true)
                .WithCancellation(token);
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace StackBuild.Goal
{
    public class GoalCore : MonoBehaviour
    {
        [SerializeField] private GameObject collisionObject;
        [SerializeField] private GameObject stackPrefab;
        [SerializeField] private Transform stackPos;
        [SerializeField] private float widthSize = 10f;
        [SerializeField] private float heightSize = 1f;
        [SerializeField] private int column = 1;
        [SerializeField] private int row = 1;
        [SerializeField] private int maxHeight;

        private Queue<PartsMesh> stackQueue = new Queue<PartsMesh>();
        private GameObject buildingBase;
        private int height = 0;
        private int totalHeight = 0;
        private int floorPartsCount = 0;

        private void Start()
        {
            var token = this.GetCancellationTokenOnDestroy();

            buildingBase = Instantiate(stackPrefab, transform);
            buildingBase.transform.position += Vector3.down * -0.5f;

            collisionObject.OnTriggerEnterAsObservable()
                .Where(x => x.gameObject.CompareTag("Parts"))
                .Select(x => x.gameObject.GetComponent<PartsMesh>())
                .Where(x => x.MeshId != -1)
                .Subscribe(parts => stackQueue.Enqueue(parts));

            StackLookAsync(token);
        }

        private async void StackLookAsync(CancellationToken token = default)
        {
            while (!token.IsCancellationRequested)
            {
                while (stackQueue.TryDequeue(out var parts))
                {
                    await StackAsync(stackQueue.Dequeue(), token);
                    await BaseDownAsync(token);
                    await UniTask.Delay(TimeSpan.FromSeconds(0.05f), cancellationToken: token);
                }
            }
        }

        private async UniTask StackAsync(PartsMesh parts, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();

            var obj = Instantiate(stackPrefab, buildingBase.transform);
            if (!obj.TryGetComponent(out MeshRenderer renderer)) return;

            obj.transform.localPosition = new Vector3(
                x: (row - 1 - floorPartsCount / column) * (widthSize / row) + (widthSize / row) - (widthSize / 2),
                y: height + 20,
                z: (floorPartsCount % column) * (widthSize / column) + (widthSize / column) - (widthSize / 2));
            obj.transform.localScale = new Vector3(widthSize / row, heightSize, widthSize / column);

            renderer.sharedMaterial = parts.SharedMaterial;
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
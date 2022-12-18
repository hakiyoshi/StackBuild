using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using StackBuild.Game;
using UniRx;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

namespace StackBuild
{
    public class BuildingCore : NetworkBehaviour
    {
        [SerializeField] private BuildSettings settings;
        [SerializeField] private MatchControl matchControl;
        [SerializeField] private Transform stackPos;

        private Queue<BuildCubeId> stackQueue = new();
        private GameObject buildingBase;
        private float height = 0;
        private float totalHeight = 0;
        private int floorPartsCount = 0;
        private CancellationTokenSource cts;
        private float timeMultiplier = 1.0f;

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

            matchControl.State.Subscribe(state =>
            {
                if (state == MatchState.Starting)
                {
                    timeMultiplier = 1.0f;
                    StopStackTimer();
                    StartStackTimer();
                }
                else if (state == MatchState.Finished)
                {
                    timeMultiplier = 0.1f;
                    Finished();
                }
            }).AddTo(this);
        }

        private void StartStackTimer()
        {
            StopStackTimer();

            cts = new();
            cts.AddTo(this);
            StackTimerAsync(cts.Token).Forget();
        }

        private void StopStackTimer()
        {
            if (cts == null) return;

            cts.Cancel();
            cts.Dispose();
            cts = null;
        }

        private async UniTask StackTimerAsync(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            while (!token.IsCancellationRequested)
            {
                while (stackQueue.TryDequeue(out var buildingId))
                {
                    StackAsync(settings.BuildingDataDictionary[buildingId], token).Forget();
                    await BaseDownAsync(token);
                    await UniTask.Delay(TimeSpan.FromSeconds(LoopTime * timeMultiplier), cancellationToken: token);
                }
                await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken: token);
            }
        }

        private async UniTask StackAsync(BuildCubeData data, CancellationToken token)
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

            await obj.transform.DOLocalMoveY(-20, PartsFallTime * timeMultiplier)
                .SetEase(Ease.InCubic)
                .SetRelative(true)
                .WithCancellation(token);
        }

        private async UniTask BaseDownAsync(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            if (height <= MaxHeight) return;

            totalHeight += MaxHeight;
            height -= MaxHeight;

            await buildingBase.transform.DOLocalMoveY(-MaxHeight, BaseFallTime * timeMultiplier)
                .SetEase(Ease.OutBack)
                .SetDelay(PartsFallTime)
                .SetRelative(true)
                .WithCancellation(token);
        }

        [ServerRpc(RequireOwnership = false)]
        public void FinishedServerRpc()
        {
            if (!IsServer) return;

            FinishedClientRpc();
        }

        [ClientRpc]
        private void FinishedClientRpc()
        {
            if (IsOwner) return;

            FinishedImpl();
        }

        private void Finished()
        {
            if (IsSpawned && !IsOwner)
            {
                FinishedServerRpc();
            }
            else
            {
                FinishedImpl();
            }
        }

        private void FinishedImpl()
        {
            buildingBase.transform.DOLocalMoveY(0, settings.FinishedUpTime).SetEase(Ease.OutElastic);
        }
    }
}
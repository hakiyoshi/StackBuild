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
        private readonly ReactiveProperty<float> totalHeight = new(0);
        private int floorPartsCount = 0;
        private bool isFinished = false;
        private CancellationTokenSource cts;

        private float WidthSize => settings.WidthSize;
        private float HeightSize => settings.HeightSize;
        private int Column => settings.Column;
        private int Row => settings.Row;
        private float MaxHeight => settings.MaxHeight;
        private float PartsFallTime => settings.PartsFallTime;
        private float BaseFallTime => settings.BaseFallTime;
        private float LoopTime => settings.LoopTime;
        public IReadOnlyReactiveProperty<float> TotalHeight => totalHeight;

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
                    isFinished = false;
                    StopStackTimer();
                    StartStackTimer();
                }
                else if (state == MatchState.Finished)
                {
                    isFinished = true;
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
                    if (isFinished) continue;
                    await UniTask.Delay(TimeSpan.FromSeconds(LoopTime), cancellationToken: token);
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
                y: totalHeight.Value + 20,
                z: (floorPartsCount % Column) * (WidthSize / Column) + (WidthSize / Column) - (WidthSize / 2));
            obj.transform.localScale = new Vector3(WidthSize / Row, HeightSize, WidthSize / Column);

            var row = (floorPartsCount / Column);
            var col = (floorPartsCount % Column);
            var euler = new Vector3(-90, 0, 0);

            obj.name = $"{row} {col}";
            if (col == Column - 1 && row != Row - 1)
            {
                euler.z = 0;
            }
            else if (row == 0 && col != Column - 1)
            {
                euler.z = 90;
            }
            else if (col == 0 && col != Column - 1)
            {
                euler.z = 180;
            }
            else if (row == Row - 1 && col != 0)
            {
                euler.z = 270;
            }
            obj.transform.localRotation = Quaternion.Euler(euler);

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
                totalHeight.Value += HeightSize;
                floorPartsCount = 0;
            }

            await obj.transform.DOLocalMoveY(-20, PartsFallTime)
                .SetEase(Ease.InCubic)
                .SetRelative(true)
                .WithCancellation(token);
        }

        private async UniTask BaseDownAsync(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            if (height <= MaxHeight) return;

            height -= MaxHeight;

            if (isFinished) return;

            await buildingBase.transform.DOLocalMoveY(-MaxHeight, BaseFallTime)
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
            buildingBase.transform.DOLocalMoveY(0, settings.FinishedUpTime).SetEase(settings.FinishedUpEase);
        }
    }
}
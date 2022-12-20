using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using StackBuild.Game;
using UniRx;
using Unity.Netcode;
using UnityEngine;

namespace StackBuild
{
    public class CoverCore : NetworkBehaviour
    {
        [SerializeField] private CoverSettings settings;
        [SerializeField] private MatchControl matchControl;

        private MeshRenderer meshRenderer = null;
        private MeshCollider meshCollider = null;

        private ReactiveProperty<bool> isOpen = new();
        public IReadOnlyReactiveProperty<bool> IsOpen => isOpen;

        private CancellationTokenSource cts = new();

        private void Awake()
        {
            TryGetComponent(out meshRenderer);
            TryGetComponent(out meshCollider);
        }

        private void Start()
        {
            if (meshRenderer == null || meshCollider == null) return;

            isOpen.Subscribe(value =>
            {
                meshRenderer.enabled = !value;
                meshCollider.enabled = !value;
            }).AddTo(this);

            matchControl.State.Subscribe(state =>
            {
                if (state == MatchState.Ingame)
                {
                    StartCoverLoop();
                }
                else
                {
                    StopCoverLoop();
                }
            }).AddTo(this);

            SetCoverOpen(false);
        }

        public void StartCoverLoop()
        {
            if (meshRenderer == null || meshCollider == null) return;

            StopCoverLoop();
            cts = new();
            cts.AddTo(this);

            ClientSyncedCoverCloseOpen(cts.Token).Forget();
        }

        public void StopCoverLoop()
        {
            if (cts == null) return;

            cts.Cancel();
            cts.Dispose();
            cts = null;
        }

        private void SetCoverOpen(bool value)
        {
            isOpen.Value = value;
        }

        private void ToggleCoverOpen()
        {
            if (IsSpawned && IsServer)
            {
                CoverSyncedServerRpc(NetworkManager.LocalTime.Time);
            }
            else
            {
                ToggleCoverOpenImpl();
            }
        }

        private void ToggleCoverOpenImpl()
        {
            isOpen.Value = !isOpen.Value;
        }

        private async UniTaskVoid WaitAndToggleCover(float timeToWait, CancellationToken token)
        {
            if (timeToWait > 0)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(timeToWait), cancellationToken: token);
            }

            ToggleCoverOpenImpl();
            //Debug.LogError("Cover: " + (meshRenderer.enabled ? "閉じてる" : "空いてる") + $"\nTime: {NetworkManager.LocalTime.Time - timeToWait}");
        }

        private async UniTask ClientSyncedCoverCloseOpen(CancellationToken token)
        {
            SetCoverOpen(false);
            while (true)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(settings.AppearanceTime), cancellationToken: token);
                ToggleCoverOpen();
                await UniTask.Delay(TimeSpan.FromSeconds(settings.IntervalTime), cancellationToken: token);
                ToggleCoverOpen();
            }
        }

        [ServerRpc]
        private void CoverSyncedServerRpc(double time)
        {
            if (!IsServer) return;

            CoverSyncedClientRpc(time);
            var timeToWait = time - NetworkManager.ServerTime.Time;
            WaitAndToggleCover((float) timeToWait, this.GetCancellationTokenOnDestroy()).Forget();
        }

        [ClientRpc]
        private void CoverSyncedClientRpc(double time)
        {
            if (IsOwner) return;

            var timeToWait = time - NetworkManager.ServerTime.Time;
            WaitAndToggleCover((float)timeToWait, this.GetCancellationTokenOnDestroy()).Forget();
        }
    }
}
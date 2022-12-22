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

        private ReactiveProperty<bool> isOpened = new();
        public IReadOnlyReactiveProperty<bool> IsOpened => isOpened;

        private CancellationTokenSource cts = null;

        private void Awake()
        {
            TryGetComponent(out meshRenderer);
            TryGetComponent(out meshCollider);
        }

        private void Start()
        {
            if (meshRenderer == null || meshCollider == null) return;

            isOpened.Subscribe(value =>
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

        public override void OnNetworkSpawn()
        {
            SetCoverOpen(false);
            StopCoverLoop();

            if (IsOwner)
            {
                StartCoverLoop();
            }
        }

        public override void OnNetworkDespawn()
        {

        }

        public void StartCoverLoop()
        {
            if (meshRenderer == null || meshCollider == null) return;

            StopCoverLoop();
            cts = new();
            cts.AddTo(this);

            ClientSyncedCover(cts.Token).Forget();
        }

        public void StopCoverLoop()
        {
            if (cts == null) return;

            cts.Cancel();
            cts.Dispose();
            cts = null;
        }

        private void SetCoverOpen(bool isOpen)
        {
            if (IsSpawned && IsOwner)
            {
                CoverSyncedServerRpc(NetworkManager.LocalTime.Time, isOpen);
            }
            else
            {
                SetCoverOpenCore(isOpen);
            }
        }

        private void SetCoverOpenCore(bool isOpen)
        {
            isOpened.Value = isOpen;
        }

        private async UniTask ClientSyncedCover(CancellationToken token)
        {
            while (true)
            {
                SetCoverOpen(false);
                await UniTask.Delay(TimeSpan.FromSeconds(settings.AppearanceTime), cancellationToken: token);
                SetCoverOpen(true);
                await UniTask.Delay(TimeSpan.FromSeconds(settings.IntervalTime), cancellationToken: token);
            }
        }

        [ServerRpc]
        private void CoverSyncedServerRpc(double time, bool isOpen)
        {
            if (!IsServer) return;

            CoverSyncedClientRpc(time, isOpen);
            var timeToWait = time - NetworkManager.ServerTime.Time;
            WaitAndSetCoverOpen((float)timeToWait, isOpen, this.GetCancellationTokenOnDestroy()).Forget();
        }

        [ClientRpc]
        private void CoverSyncedClientRpc(double time, bool isOpen)
        {
            if (IsOwner) return;

            var timeToWait = time - NetworkManager.ServerTime.Time;
            WaitAndSetCoverOpen((float)timeToWait, isOpen, this.GetCancellationTokenOnDestroy()).Forget();
        }

        private async UniTaskVoid WaitAndSetCoverOpen(float timeToWait, bool isOpen, CancellationToken token)
        {
            if (timeToWait > 0)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(timeToWait), cancellationToken: token);
            }

            SetCoverOpenCore(isOpen);
            //Debug.LogError("Cover: " + (meshRenderer.enabled ? "閉じてる" : "空いてる") + $"\nTime: {NetworkManager.LocalTime.Time - timeToWait}");
        }
    }
}
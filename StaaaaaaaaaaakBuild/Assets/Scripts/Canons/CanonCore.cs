using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using Unity.Netcode;
using UnityEngine;

namespace StackBuild
{
    public class CanonCore : MonoBehaviour
    {
        public float ShootPower;
        public float ShootAngle;

        [SerializeField] private Transform shootTarget;

        private Queue<PartsCore> queue = new Queue<PartsCore>();

        private void Start()
        {
            ShootTimerAsync(this.GetCancellationTokenOnDestroy()).Forget();
        }

        private async UniTaskVoid ShootTimerAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                if (queue.TryDequeue(out var parts) &&
                    parts.gameObject.TryGetComponent(out Rigidbody rb))
                {
                    var shootPos = shootTarget.position;
                    var sub = shootPos - transform.position;
                    var force = sub.normalized * ShootPower;

                    rb.velocity = Vector3.zero;
                    rb.position = shootPos;

                    parts.Show();

                    rb.AddForce(force, ForceMode.VelocityChange);
                }

                await UniTask.Delay(TimeSpan.FromSeconds(0.07f), cancellationToken: token);
            }
        }

        public void Enqueue(PartsCore parts)
        {
            queue.Enqueue(parts);
        }
    }
}
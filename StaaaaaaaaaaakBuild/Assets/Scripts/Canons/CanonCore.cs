using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UniRx.Triggers;
using Unity.Netcode;
using UnityEngine;

namespace StackBuild
{
    public class CanonCore : MonoBehaviour
    {
        [SerializeField] private CanonSettings settings;
        [SerializeField] private Transform shootTarget;

        private Queue<PartsCore> queue = new Queue<PartsCore>();

        private void Start()
        {
            Observable.Interval(TimeSpan.FromSeconds(settings.ShootDuration))
                .Where(_ => queue.Count > 0)
                .Select(_ => queue.Dequeue().GetComponent<PartsPhysics>())
                .Subscribe(partsPhysics =>
                {
                    var shootPos = shootTarget.position;
                    var sub = shootPos - transform.position;
                    var force = sub.normalized * settings.ShootPower;

                    partsPhysics.Teleport(shootPos);
                    partsPhysics.PartsCore.Show();
                    partsPhysics.AddForce(force, ForceMode.VelocityChange);
                }).AddTo(this);
        }

        public void Enqueue(PartsCore parts)
        {
            queue.Enqueue(parts);
        }
    }
}
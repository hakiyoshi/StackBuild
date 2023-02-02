using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using StackBuild.Audio;
using UniRx;
using UniRx.Triggers;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

namespace StackBuild
{
    public class GoalCore : NetworkBehaviour
    {
        [SerializeField] private GoalSettings settings;
        [SerializeField] private BuildingCore buildingCore;
        [SerializeField] private GameObject collisionObject;

        [Header("Audio")]
        [SerializeField] private AudioSourcePool audioSourcePool;
        [SerializeField] private AudioCue enderThePortalCue;

        private FloorData[] FloorDataArray => settings.FloorDataList.ToArray();
        private Dictionary<MaterialId, int> partsCount = new();

        private void Start()
        {
            foreach (var id in (MaterialId[])Enum.GetValues(typeof(MaterialId)))
            {
                partsCount[id] = 0;
            }

            collisionObject.OnTriggerEnterAsObservable()
                .Where(x => x.gameObject.CompareTag("Parts"))
                .Select(x => x.gameObject.GetComponent<PartsNetworkSync>())
                .Where(x => !x.IsSpawned || NetworkManager.Singleton != null && NetworkManager.Singleton.IsServer)
                .Select(x => x.gameObject.GetComponent<PartsCore>())
                .Subscribe(Goal).AddTo(this);
        }

        private void Goal(PartsCore parts)
        {
            audioSourcePool.Rent(enderThePortalCue).PlayAndReturnWhenStopped();

            PartsNetworkSync networkSync = parts.GetComponent<PartsNetworkSync>();

            foreach (var buildMaterial in parts.GetPartsData().containsMaterials)
            {
                partsCount[buildMaterial.Key] += buildMaterial.Value;
            }

            for (int i = 0; i < FloorDataArray.Length; i++)
            {
                var data = FloorDataArray[i];

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

                if (networkSync.IsSpawned)
                {
                    networkSync.LostOwnershipServerRpc(NetworkManager.ServerClientId);
                    EnqueueServerRpc(i);
                }
                else
                {
                    Enqueue(i);
                }
            }
        }

        private void Enqueue(int index)
        {
            foreach (var id in FloorDataArray[index].buildingIds)
            {
                buildingCore.Enqueue(id);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void EnqueueServerRpc(int index)
        {
            EnqueueClientRpc(index);
        }

        [ClientRpc]
        private void EnqueueClientRpc(int index)
        {
            Enqueue(index);
        }
    }
}
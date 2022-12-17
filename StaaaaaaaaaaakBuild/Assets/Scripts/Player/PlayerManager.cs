using System;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

namespace StackBuild
{
    public class PlayerManager : NetworkBehaviour
    {
        [field: SerializeField] public GameObject[] PlayerObjects { get; private set; } = Array.Empty<GameObject>();
        [SerializeField] private PlayerManagerProperty playerManagerProperty;

        private void Start()
        {
            if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsServer)
                OwnerAllocation();

            playerManagerProperty.playerManager = this;
        }

        public override void OnNetworkSpawn()
        {
            if(!IsServer)
                return;

            NetworkManager.Singleton.OnClientConnectedCallback += OwnerAllocation;
        }

        public override void OnNetworkDespawn()
        {
            if (!IsServer)
                return;

            NetworkManager.Singleton.OnClientConnectedCallback -= OwnerAllocation;
        }

        void OwnerAllocation(ulong id = 0)
        {
            var clientIds = NetworkManager.Singleton.ConnectedClientsIds;

            for (var i = 0; i < clientIds.Count; i++)
            {
                if(PlayerObjects.Length <= i)
                    return;

                if (!PlayerObjects[i].TryGetComponent(out NetworkObject networkObject))
                    continue;

                //オーナーを変更する
                if (networkObject.IsSpawned)
                    networkObject.ChangeOwnership(clientIds[i]);
                else
                    networkObject.SpawnWithOwnership(clientIds[i]);
            }
        }

        public int GetPlayerIndex(GameObject playerObject)
        {
            var index = Array.IndexOf(PlayerObjects, playerObject);
            Debug.Assert(index != -1);
            return index;
        }

        public static GameObject GetPlayerObject(Transform playerChild)
        {
            var layerMask = LayerMask.GetMask("P1", "P2");
            while (playerChild != null)
            {
                if ((1 << playerChild.gameObject.layer & layerMask) != 0)
                    return playerChild.gameObject;

                playerChild = playerChild.parent;
            }

            return null;
        }
    }
}

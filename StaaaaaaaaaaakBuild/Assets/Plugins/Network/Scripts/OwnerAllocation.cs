using System;
using System.Collections;
using System.Collections.Generic;
using NetworkSystem;
using UniRx;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NetworkSystem
{
    public class OwnerAllocation : NetworkBehaviour
    {
        [SerializeField] private NetworkObject[] playerObjects = Array.Empty<NetworkObject>();

        private void Start()
        {
            ObjectOwnerAllocation();
        }

        void ObjectOwnerAllocation()
        {
            if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsServer)
                return;

            var clients = NetworkManager.Singleton.ConnectedClients;

            int playerIndex = 0;
            foreach (var client in clients)
            {
                if(playerIndex > playerObjects.Length)
                    continue;
                
                if (IsSpawned)
                    playerObjects[playerIndex].ChangeOwnership(client.Value.ClientId);
                else
                    playerObjects[playerIndex].SpawnWithOwnership(client.Value.ClientId);
                
                playerIndex++;
            }
        }
    }
}

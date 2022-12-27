using Unity.Netcode;
using UnityEngine;

namespace StackBuild.MenuNetwork
{
    public class PlayerPropertyOperator : NetworkBehaviour
    {
        [field: SerializeField] public PlayerProperty[] playerPropertys { get; private set; }
        [field: SerializeField] public CharacterProperty[] characterProperties { get; private set; }

        private const int INVALID = -1;
        private int clientIndex = INVALID;
        private int useIndex = 0;

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                NetworkManager.OnClientConnectedCallback += SendClientIndex;
                NetworkManager.OnClientDisconnectCallback += ClientDisconnect;
            }

            if (IsHost)
            {
                SendClientIndex(NetworkManager.LocalClientId);
            }
        }

        public override void OnNetworkDespawn()
        {
            if (IsServer)
            {
                NetworkManager.OnClientConnectedCallback -= SendClientIndex;
                NetworkManager.OnClientDisconnectCallback -= ClientDisconnect;
                useIndex = 0;
                clientIndex = INVALID;
            }
        }

        void ClientDisconnect(ulong index)
        {
            useIndex = System.Math.Max(useIndex - 1, 0);
        }

        void SendClientIndex(ulong index)
        {
            //送信するクライアント指定
            var clientRpcParams = new ClientRpcParams()
            {
                Send = new ClientRpcSendParams()
                {
                    TargetClientIds = new ulong[] {index}
                }
            };

            //インデックスを割り当てて送信
            SendClientIndexClientRpc(useIndex, clientRpcParams);
            useIndex++;
        }

        [ClientRpc]
        private void SendClientIndexClientRpc(int index, ClientRpcParams clientRpcParams = default)
        {
            clientIndex = index;
        }

        [ServerRpc(RequireOwnership = false)]
        private void SendSelectedCharacterServerRpc(int index, int characterType)
        {
            if (!IsServer)
                return;

            if(index == INVALID)
            {
                Debug.LogError("UseIndex of MenuNetwork is invalid.");
                return;
            }

            SetCharacterProperty(index, characterType);
            SendSelectedCharacterClientRpc(index, characterType);
        }

        [ClientRpc]
        private void SendSelectedCharacterClientRpc(int index, int characterType)
        {
            SetCharacterProperty(index, characterType);
        }

        //選択したキャラを送信する
        //characterTypeはcharacterPropertiesのインデックス
        public void ChangeSelectedCharacter(int playerIndex, int characterType)
        {
            if (IsSpawned)
            {
                SendSelectedCharacterServerRpc(clientIndex, characterType);
            }
            else
            {
                SetCharacterProperty(playerIndex, characterType);
            }
        }

        //キャラクタープロパティをセットする
        public void SetCharacterProperty(int playerIndex, int characterType)
        {
            playerPropertys[playerIndex].Initialize(characterProperties[characterType]);
        }
    }
}
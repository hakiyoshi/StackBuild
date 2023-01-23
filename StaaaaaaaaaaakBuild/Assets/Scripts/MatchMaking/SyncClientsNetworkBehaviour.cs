using Unity.Netcode;

namespace StackBuild.MatchMaking
{
    public class SyncClientsNetworkBehaviourNetworkBehaviour : NetworkBehaviour, ISyncClientsNetworkBehaviour
    {
        protected int connectedClientCount { get; private set; } = 0;
        protected int readyClientCount { get; private set; } = 0;

        public override void OnNetworkSpawn()
        {
            SendSpawnServerRpc();
        }

        public override void OnNetworkDespawn()
        {
            SendDespawnServerRpc();
        }

        [ServerRpc(RequireOwnership = false)]
        protected void SendSpawnServerRpc()
        {
            if (!IsServer)
                return;

            connectedClientCount++;
            SetSpawnCountClientRpc(connectedClientCount);
            OnClientSpawn(connectedClientCount);
        }

        [ServerRpc(RequireOwnership = false)]
        protected void SendDespawnServerRpc()
        {
            if (!IsServer)
                return;

            connectedClientCount--;
            SetSpawnCountClientRpc(connectedClientCount);
        }

        [ServerRpc(RequireOwnership = false)]
        protected void SendReadyServerRpc()
        {
            if (!IsServer)
                return;

            readyClientCount++;
            SetReadyCountClientRpc(readyClientCount);
            OnClientReady(readyClientCount);
        }

        [ServerRpc]
        protected void SendUnreadyServerRpc()
        {
            if (!IsServer)
                return;

            readyClientCount--;
            SetReadyCountClientRpc(readyClientCount);
            OnClientDespawn(connectedClientCount);
        }

        [ClientRpc]
        protected void SetSpawnCountClientRpc(int count)
        {
            connectedClientCount = count;
        }

        [ClientRpc]
        protected void SetReadyCountClientRpc(int count)
        {
            readyClientCount = count;
        }

        protected void ResetParameters()
        {
            connectedClientCount = 0;
            readyClientCount = 0;
            SetSpawnCountClientRpc(0);
            SetReadyCountClientRpc(0);
        }

        protected virtual void OnClientSpawn(int count)
        {

        }

        protected virtual void OnClientDespawn(int count)
        {

        }

        protected virtual void OnClientReady(int count)
        {

        }

        protected virtual void OnClientUnready(int count)
        {

        }
    }

    public interface ISyncClientsNetworkBehaviour
    {

    }
}
using Unity.Netcode;

namespace StackBuild.MatchMaking
{
    public class SyncClientsNetworkBehaviour : NetworkBehaviour, ISyncClientsNetworkBehaviour
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
            SendSpawnClientRpc(connectedClientCount);
            OnClientSpawn(connectedClientCount);
        }

        [ServerRpc(RequireOwnership = false)]
        protected void SendDespawnServerRpc()
        {
            if (!IsServer)
                return;

            connectedClientCount--;
            SendDespawnClientRpc(connectedClientCount);
            OnClientDespawn(connectedClientCount);
        }

        [ServerRpc(RequireOwnership = false)]
        protected void SendReadyServerRpc()
        {
            if (!IsServer)
                return;

            readyClientCount++;
            SendReadyClientRpc(readyClientCount);
            OnClientReady(connectedClientCount);
        }

        [ServerRpc(RequireOwnership = false)]
        protected void SendUnreadyServerRpc()
        {
            if (IsServer)
                return;

            readyClientCount--;
            SendUnreadyClientRpc(readyClientCount);
            OnClientUnready(readyClientCount);
        }

        [ClientRpc]
        private void SendSpawnClientRpc(int count)
        {
            if (IsServer)
                return;

            connectedClientCount = count;
            OnClientSpawn(count);
        }

        [ClientRpc]
        private void SendDespawnClientRpc(int count)
        {
            if (IsServer)
                return;

            connectedClientCount = count;
            OnClientDespawn(count);
        }

        [ClientRpc]
        private void SendReadyClientRpc(int count)
        {
            if (IsServer)
                return;

            readyClientCount = count;
            OnClientReady(count);
        }

        [ClientRpc]
        private void SendUnreadyClientRpc(int count)
        {
            readyClientCount = count;
            OnClientUnready(count);
        }

        [ServerRpc]
        protected void ResetParametersServerRpc()
        {
            if (!IsServer) return;

            ResetParameters();
        }

        [ClientRpc]
        private void ResetParametersClientRpc()
        {
            if (IsServer) return;

            ResetParameters();
        }

        protected void ResetParameters()
        {
            connectedClientCount = 0;
            readyClientCount = 0;
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
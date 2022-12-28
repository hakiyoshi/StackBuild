using Unity.Netcode;

namespace StackBuild.Game
{
    public class SyncWaitingSystem : NetworkBehaviour
    {
        private int numWaitingToSignal = 0;

        [ServerRpc(RequireOwnership = false)]
        protected void SendStandbyServerRpc()
        {
            if (!IsServer)
                return;

            numWaitingToSignal++;
            OnSendStandby(numWaitingToSignal);
        }

        [ServerRpc(RequireOwnership = false)]
        protected void SendStandbyReleaseServerRpc()
        {
            if (!IsServer)
                return;

            numWaitingToSignal++;
            OnSendStandbyRelease(numWaitingToSignal);
        }

        protected void ResetNumWaitingToSignal()
        {
            numWaitingToSignal = 0;
        }

        protected virtual void OnSendStandby(int numWaitingToSignal)
        {

        }

        protected virtual void OnSendStandbyRelease(int numWaitingToSignal)
        {

        }
    }
}
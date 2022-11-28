using UniRx;
using Unity.Netcode;
using UnityEngine;
using VContainer;

namespace StackBuild
{
    [RequireComponent(typeof(NetworkObject))]
    public class PartsCore : NetworkBehaviour
    {
        [SerializeField] private InGameSettings settings;
        public InGameSettings Settings => settings;
        private ReactiveProperty<bool> isActive = new ReactiveProperty<bool>();
        public IReactiveProperty<bool> IsActive => isActive;

        private NetworkObject networkObject;

        public PartsCore Spawn()
        {
            isActive.Value = true;

            networkObject = GetComponent<NetworkObject>();

            /*
            if (!IsLocalPlayer)
            {
                networkObject.Spawn();
            }
            */
            return this;
        }

        public void Despawn()
        {
            isActive.Value = false;

            /*
            if (IsLocalPlayer) return;
            networkObject.Despawn();
            */
        }

        public void Show()
        {
            isActive.Value = true;

            /*
            if (IsLocalPlayer) return;
            foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds)
            {
                networkObject.NetworkShow(clientId);
            }
            */
        }

        public void Hide()
        {
            isActive.Value = false;

            /*
            if (IsLocalPlayer) return;
            foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds)
            {
                networkObject.NetworkHide(clientId);
            }
            */
        }
    }
}

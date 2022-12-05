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
            return this;
        }

        public void Despawn()
        {
            isActive.Value = false;
        }

        public void Show()
        {
            isActive.Value = true;
        }

        public void Hide()
        {
            isActive.Value = false;
        }
    }
}

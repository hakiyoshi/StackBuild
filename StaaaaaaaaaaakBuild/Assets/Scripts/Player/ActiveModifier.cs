using System;
using Unity.Netcode;
using UnityEngine;

namespace StackBuild
{
    public class ActiveModifier : NetworkBehaviour
    {
        [Serializable]
        struct ChangeMonoBehaviour
        {
            public MonoBehaviour monoBehaviour;
            public bool ActiveOnline;
        }

        [SerializeField] private ChangeMonoBehaviour[] changeMonoBehaviours = Array.Empty<ChangeMonoBehaviour>();

        private void Start()
        {
            ChangeActive(false);
        }

        public override void OnNetworkSpawn()
        {
            ChangeActive(true);
        }

        public override void OnNetworkDespawn()
        {
            ChangeActive(false);
        }

        void ChangeActive(bool isOnline)
        {
            foreach (var obj in changeMonoBehaviours)
            {
                obj.monoBehaviour.enabled = !(obj.ActiveOnline ^ isOnline);
            }
        }
    }
}
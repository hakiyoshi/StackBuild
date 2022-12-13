using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace StackBuild
{
    public class CanonManager : NetworkBehaviour
    {
        [SerializeField] private List<CanonCore> canonList;

        public int GetRandomIndex()
        {
            return Random.Range(0, canonList.Count);
        }

        public void RandomEnqueue(PartsCore[] parts)
        {
            int index = GetRandomIndex();

            for (int i = 0; i < parts.Length; i++)
            {
                Enqueue(index, parts[i]);
            }
        }

        public void RandomEnqueue(PartsCore parts)
        {
            Enqueue(GetRandomIndex(), parts);
        }

        [ServerRpc(RequireOwnership = true)]
        public void Enqueue(int index, PartsCore parts) => EnqueueImpl(index, parts);

        [ClientRpc]
        private void EnqueueImpl(int index, PartsCore parts)
        {
            canonList[index].Enqueue(parts);
        }
    }
}

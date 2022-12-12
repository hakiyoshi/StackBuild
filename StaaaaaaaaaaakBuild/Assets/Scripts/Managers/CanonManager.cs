using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StackBuild
{
    public class CanonManager : MonoBehaviour
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

        public void Enqueue(int index, PartsCore parts)
        {
            canonList[index].Enqueue(parts);
        }
    }
}

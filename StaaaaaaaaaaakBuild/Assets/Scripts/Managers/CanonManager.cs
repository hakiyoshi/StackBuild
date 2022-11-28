using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StackBuild
{
    public class CanonManager : MonoBehaviour
    {
        [SerializeField] private List<CanonCore> canonList;

        public void RandomEnqueue(PartsCore[] parts)
        {
            int index = Random.Range(0, canonList.Count);

            for (int i = 0; i < parts.Length; i++)
            {
                Enqueue(index, parts[i]);
            }
        }

        public void RandomEnqueue(PartsCore parts)
        {
            Enqueue(Random.Range(0, canonList.Count), parts);
        }

        public void Enqueue(int index, PartsCore parts)
        {
            canonList[index].Enqueue(parts);
        }
    }
}

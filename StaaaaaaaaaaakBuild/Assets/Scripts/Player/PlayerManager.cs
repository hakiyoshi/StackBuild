using System;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

namespace StackBuild
{
    public class PlayerManager : MonoBehaviour
    {
        [field: SerializeField] public GameObject[] PlayerObjects { get; private set; } = Array.Empty<GameObject>();

        public int GetPlayerIndex(GameObject playerObject)
        {
            var index = Array.IndexOf(PlayerObjects, playerObject);
            Debug.Assert(index != -1);
            return index;
        }
    }
}

using System;
using Unity.VisualScripting;
using UnityEngine;

namespace StackBuild
{
    public class PlayerManager : MonoBehaviour
    {
        [SerializeField] private GameObject[] playerObjects = Array.Empty<GameObject>();

        public int GetPlayerIndex(GameObject playerObject)
        {
            return Array.IndexOf(playerObjects, playerObject);
        }
    }
}

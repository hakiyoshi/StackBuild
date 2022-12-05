using System;
using UnityEngine;

namespace StackBuild
{
    public class PlayerSetup : MonoBehaviour
    {
        [SerializeField] private PlayerProperty playerProperty;

        private void Start()
        {
            playerProperty.PlayerObject = this.gameObject;
        }
    }
}
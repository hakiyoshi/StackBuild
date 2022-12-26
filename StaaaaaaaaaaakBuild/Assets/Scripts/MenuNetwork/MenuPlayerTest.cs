using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace StackBuild.MenuNetwork
{
    public class MenuPlayerTest : NetworkBehaviour
    {
        [SerializeField] private PlayerPropertyOperator menuNetwork;

        [SerializeField] private int playerIndex;
        [SerializeField] private int characterType;

        private void Update()
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                menuNetwork.ChangeSelectedCharacter(playerIndex, characterType);
            }
        }
    }
}
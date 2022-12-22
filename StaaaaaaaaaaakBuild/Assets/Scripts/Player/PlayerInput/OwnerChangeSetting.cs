using System;
using StackBuild.Game;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace StackBuild
{
    public class OwnerChangeSetting : NetworkBehaviour
    {
        [SerializeField] private PlayerInputProperty playerInputProperty;
        [SerializeField] private PlayerManager playerManager;

        private int playerIndex
        {
            get
            {
                return playerManager.GetPlayerIndex(this.gameObject);
            }
        }

        PlayerInput GetInputSet()
        {
            return playerInputProperty.PlayerInputs[playerIndex];
        }

        public override void OnNetworkDespawn()
        {
            var playerInput = GetInputSet();
            if(playerInput == null)
                return;

            playerInput.gameObject.SetActive(true);
            playerInputProperty.playerInputManager.SettingPlayerDevice(playerIndex,
                playerInputProperty.DeviceIds[playerIndex]);
        }

        public override void OnGainedOwnership()
        {
            if (!IsOwner)
                return;

            var playerInput = GetInputSet();
            if(playerInput == null)
                return;

            GainedInput(playerInput);
        }

        public override void OnLostOwnership()
        {
            var playerInput = GetInputSet();
            if(playerInput == null)
                return;

            LostInput(playerInput);
        }

        void LostInput(PlayerInput playerInput)
        {
            playerInputProperty.playerInputManager.SettingPlayerDevice(playerIndex, PlayerInputProperty.UNSETID);
            playerInput.gameObject.SetActive(false);
        }

        void GainedInput(PlayerInput playerInput)
        {
            SwitchDevice(playerInput);
            playerInput.gameObject.SetActive(true);
        }

        void SwitchDevice(PlayerInput playerInput)
        {
            //１Pの設定を持ってくる
            var playerInputManager = playerInputProperty.playerInputManager;
            playerInputManager.SettingPlayerDevice(playerIndex, playerInputProperty.DeviceIds[0]);
        }
    }
}
using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace StackBuild
{
    public class OwnerChangeSetting : NetworkBehaviour
    {
        public PlayerInputProperty playerInputProperty;
        public PlayerManager playerManager;

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

        public override void OnNetworkSpawn()
        {
            var playerInput = GetInputSet();
            if (!IsOwner)
            {
                LostInput(playerInput);
            }
            else
            {
                playerInput.enabled = true;
                SwitchDevice(playerInput);
            }
        }

        public override void OnNetworkDespawn()
        {
            var playerInput = GetInputSet();
            playerInput.gameObject.SetActive(true);
            playerInputProperty.SettingPlayerDevice(playerIndex, null, false);
        }

        public override void OnGainedOwnership()
        {
            if (!IsOwner)
                return;

            var playerInput = GetInputSet();
            GainedInput(playerInput);
        }

        public override void OnLostOwnership()
        {
            var playerInput = GetInputSet();
            LostInput(playerInput);
        }

        void LostInput(PlayerInput playerInput)
        {
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
            playerInputProperty.SettingPlayerDevice(
                playerIndex,
                playerInputProperty.DeviceIds[playerIndex],
                false, false);
        }
    }
}
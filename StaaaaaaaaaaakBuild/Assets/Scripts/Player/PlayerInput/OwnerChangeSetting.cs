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

        PlayerInput GetPlayerInput()
        {
            return playerInputProperty.PlayerInputs[playerManager.GetPlayerIndex(this.gameObject)];
        }

        public override void OnNetworkSpawn()
        {
            var playerInput = GetPlayerInput();
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
            var playerIndex = playerManager.GetPlayerIndex(this.gameObject);
            var playerInput = playerInputProperty.PlayerInputs[playerIndex];

            var devices = InputSystem.devices;
            playerInput.gameObject.SetActive(true);
            SelectSwitchDevice(devices, playerInput, playerIndex);
        }

        public override void OnGainedOwnership()
        {
            var playerInput = GetPlayerInput();
            GainedInput(playerInput);
        }

        public override void OnLostOwnership()
        {
            var playerInput = GetPlayerInput();
            LostInput(playerInput);
        }

        void LostInput(PlayerInput playerInput)
        {
            playerInput.gameObject.SetActive(false);
        }

        void GainedInput(PlayerInput playerInput)
        {
            playerInput.gameObject.SetActive(true);
            SwitchDevice(playerInput);
        }

        void SwitchDevice(PlayerInput playerInput)
        {
            var devices = InputSystem.devices;
            if (SelectSwitchDevice(devices, playerInput, 0))//オンラインの場合１Pのデバイスを参照する
                return;

            //指定されていない場合は適当なやつ
            AutoSwitchDevice(devices[0], playerInput);
        }

        bool SelectSwitchDevice(in ReadOnlyArray<InputDevice> devices, PlayerInput playerInput, int deviceIndex)
        {
            var deviceid = playerInputProperty.DeviceIds[deviceIndex];
            if (deviceid == PlayerInputProperty.UNSETID)
            {
                return false;
            }

            foreach (var device in devices)
            {
                if (device.deviceId != deviceid)
                    continue;

                if ((Keyboard.current != null && Keyboard.current.device.deviceId == deviceid) ||
                    (Mouse.current != null && Mouse.current.device.deviceId == deviceid))
                {
                    playerInput.SwitchCurrentControlScheme("Keyboard&Mouse", Keyboard.current, Mouse.current);
                }
                else
                {
                    playerInput.SwitchCurrentControlScheme("Gamepad", device);
                }

                return true;
            }

            return false;
        }

        void AutoSwitchDevice(InputDevice device, PlayerInput playerInput)
        {
            if (!playerInput.user.valid)
                return;

            if ((Keyboard.current != null && Keyboard.current.device.deviceId == device.deviceId) ||
                    (Mouse.current != null && Mouse.current.device.deviceId == device.deviceId))
            {
                playerInput.SwitchCurrentControlScheme("Keyboard&Mouse", Keyboard.current, Mouse.current);
            }
            else
            {
                playerInput.SwitchCurrentControlScheme("Gamepad", device);
            }
        }
    }
}
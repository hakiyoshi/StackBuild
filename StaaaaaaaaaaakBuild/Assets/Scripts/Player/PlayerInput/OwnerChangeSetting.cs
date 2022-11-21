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

        public override void OnNetworkSpawn()
        {
            var playerInput = playerInputProperty.PlayerInputs[playerManager.GetPlayerIndex(this.gameObject)];
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

        public override void OnGainedOwnership()
        {
            if (!TryGetComponent(out PlayerInput playerInput))
                return;

            playerInput.enabled = true;
            SwitchDevice(playerInput);
        }

        public override void OnLostOwnership()
        {
            if (!TryGetComponent(out PlayerInput playerInput))
                return;

            LostInput(playerInput);
        }

        void LostInput(PlayerInput playerInput)
        {
            playerInput.user.UnpairDevices();
            playerInput.enabled = false;
        }

        void SwitchDevice(PlayerInput playerInput)
        {
            var devices = InputSystem.devices;
            if (SelectSwitchDevice(devices, playerInput))
                return;

            //指定されていない場合は適当なやつ
            AutoSwitchDevice(devices[0], playerInput);
        }

        bool SelectSwitchDevice(in ReadOnlyArray<InputDevice> devices, PlayerInput playerInput)
        {
            var deviceid = playerInputProperty.DeviceIds[0];//オンラインの場合１Pのデバイスを参照する
            if (deviceid == PlayerInputProperty.UNSETID)
                return false;

            foreach (var device in devices)
            {
                //オンラインの場合１Pのデバイスを参照する
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
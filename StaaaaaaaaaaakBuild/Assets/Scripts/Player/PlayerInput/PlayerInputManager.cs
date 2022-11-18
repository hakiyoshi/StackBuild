using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace StackBuild
{
    [RequireComponent(typeof(PlayerManager))]
    public class PlayerInputManager : MonoBehaviour
    {
        [SerializeField] private GameObject inputPrefab;
        [SerializeField] private PlayerInputProperty playerInputProperty;
        [SerializeField] private InputSender[] inputSenders = Array.Empty<InputSender>();


        private PlayerManager playerManager;

        private void Awake()
        {
            playerManager = GetComponent<PlayerManager>();

            var playerObjects = playerManager.PlayerObjects;
            for (var i = 0; i < playerObjects.Length; i++)
            {
                var parent = playerObjects[i].transform;

                if(SelectSetDevice(i, parent))
                    continue;

                AutoSetDevice(i, parent);
            }
        }

        bool SelectSetDevice(int playerIndex, Transform parent)
        {
            var deviceid = playerInputProperty.DeviceIds[playerIndex];
            if (deviceid == PlayerInputProperty.INVALID_ID)
                return false;

            foreach (var device in InputSystem.devices)
            {
                if (device.deviceId != deviceid)
                    continue;

                SettingPlayerInput(playerIndex, parent, device);
                return true;
            }

            return false;
        }

        bool AutoSetDevice(int playerIndex, Transform parent)
        {
            int deviceFlag = playerIndex;
            var devices = InputSystem.devices;
            foreach (var device in devices)
            {
                if(Mouse.current.deviceId == device.deviceId)
                    continue;

                //デバイスフラグが1以上の場合はスルー
                if(deviceFlag >= 1)
                {
                    deviceFlag--;
                    continue;
                }

                //キーボードかそれ以外で分岐
                SettingPlayerInput(playerIndex, parent, device);
                return true;
            }

            return false;
        }

        void SettingPlayerInput(int playerIndex, Transform parent, InputDevice device)
        {
            PlayerInput playerInput = null;
            if (Keyboard.current.deviceId == device.deviceId || Mouse.current.deviceId == device.deviceId)
            {
                playerInput = PlayerInput.Instantiate(inputPrefab, playerIndex: playerIndex,
                    controlScheme: "keyboard&Mouse",
                    pairWithDevices: new InputDevice[] {Keyboard.current, Mouse.current});
            }
            else
            {
                playerInput = PlayerInput.Instantiate(inputPrefab, playerIndex: playerIndex,
                    controlScheme: "Gamepad", pairWithDevice: device);
            }

            StartInputObjectSetting(playerInput, parent, playerIndex);
        }

        void StartInputObjectSetting(PlayerInput playerInput, Transform parent, int playerIndex)
        {
            playerInput.transform.parent = parent;
            playerInput.gameObject.GetComponent<Input>().inputSender = inputSenders[playerIndex];
        }
    }
}

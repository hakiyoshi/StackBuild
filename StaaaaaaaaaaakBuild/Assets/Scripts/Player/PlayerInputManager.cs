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
        [SerializeField] private InputSender[] inputSenders = Array.Empty<InputSender>();

        private PlayerManager playerManager;

        private void Awake()
        {
            playerManager = GetComponent<PlayerManager>();

            var playerObjects = playerManager.PlayerObjects;
            for (var i = 0; i < playerObjects.Length; i++)
            {
                AutoSetDevice(i, playerObjects[i].transform);
            }
        }

        void AutoSetDevice(int playerIndex, Transform parent)
        {
            int deviceFlag = playerIndex;
            var devices = InputSystem.devices;
            PlayerInput playerInput = null;
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

                if (Keyboard.current.deviceId == device.deviceId)
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
                return;
            }
            
        }

        void StartInputObjectSetting(PlayerInput playerInput, Transform parent, int playerIndex)
        {
            playerInput.transform.parent = parent;
            playerInput.gameObject.GetComponent<Input>().inputSender = inputSenders[playerIndex];
        }
    }
}

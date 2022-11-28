using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace StackBuild
{
    public class PlayerInputManager : MonoBehaviour
    {
        [SerializeField] private GameObject inputPrefab;
        [SerializeField] private PlayerInputProperty playerInputProperty;
        [SerializeField] private InputSender[] inputSenders = Array.Empty<InputSender>();
        [SerializeField] private PlayerManager playerManager;

        //管理用
        private PlayerInput[] playerInputs = new PlayerInput[PlayerInputProperty.MAX_DEVICEID];

        private void Awake()
        {
            SettingPlayerInput();
        }

        void SettingPlayerInput()
        {
            var playerObjects = playerManager.PlayerObjects;
            var devices = InputSystem.devices;
            var parent = transform.parent;
            for (var i = 0; i < playerObjects.Length; i++)
            {
                //var parent = playerObjects[i].transform;
                if(SelectSetDevice(i, parent, devices))
                    continue;

                AutoSetDevice(i, parent, devices);
            }
        }

        bool SelectSetDevice(int playerIndex, Transform parent, in ReadOnlyArray<InputDevice> devices)
        {
            var deviceid = playerInputProperty.DeviceIds[playerIndex];
            if (deviceid == PlayerInputProperty.UNSETID)
                return false;

            foreach (var device in devices)
            {
                if (device.deviceId != deviceid)
                    continue;

                SettingPlayerInput(playerIndex, parent, device);
                return true;
            }

            return false;
        }

        void AutoSetDevice(int playerIndex, Transform parent, in ReadOnlyArray<InputDevice> devices)
        {
            int deviceFlag = playerIndex;
            foreach (var device in devices)
            {
                if(Mouse.current == null || Mouse.current.deviceId == device.deviceId)
                    continue;

                //デバイスフラグが1以上の場合はスルー
                if(deviceFlag >= 1)
                {
                    deviceFlag--;
                    continue;
                }

                //キーボードかそれ以外で分岐
                SettingPlayerInput(playerIndex, parent, device);
                return;
            }

            //デバイス未設定のPlayerInput生成
            SettingPlayerInput(playerIndex, parent, null);
        }

        void SettingPlayerInput(int playerIndex, Transform parent, InputDevice device)
        {
            PlayerInput playerInput = null;
            if (device == null)
            {
                //デバイス未設定(適当なデバイスセットしてSchemeを利用していない文字列にする)
                playerInput = PlayerInput.Instantiate(inputPrefab, playerIndex: playerIndex, pairWithDevice: Mouse.current);
            }
            else if ((Keyboard.current != null && Keyboard.current.deviceId == device.deviceId) ||
                     (Mouse.current != null && Mouse.current.deviceId == device.deviceId))
            {
                //キーボード、マウス
                playerInput = PlayerInput.Instantiate(inputPrefab, playerIndex: playerIndex,
                    controlScheme: "keyboard&Mouse",
                    pairWithDevices: new InputDevice[] {Keyboard.current, Mouse.current});
            }
            else
            {
                //ゲームパッド
                playerInput = PlayerInput.Instantiate(inputPrefab, playerIndex: playerIndex,
                    controlScheme: "Gamepad", pairWithDevice: device);
            }

            playerInputs[playerIndex] = playerInput;
            StartInputObjectSetting(playerInput, parent, playerIndex);
        }

        void StartInputObjectSetting(PlayerInput playerInput, Transform parent, int playerIndex)
        {
            playerInput.transform.parent = parent;
            playerInput.gameObject.GetComponent<Input>().inputSender = inputSenders[playerIndex];
            playerInputProperty.PlayerInputs[playerIndex] = playerInput;
        }
    }
}

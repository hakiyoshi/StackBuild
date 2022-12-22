using System;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.InputSystem.Utilities;
using Observable = UniRx.Observable;

namespace StackBuild
{
    public class PlayerInputManager : MonoBehaviour
    {
        [SerializeField] private GameObject inputPrefab;
        [SerializeField] private PlayerInputProperty playerInputProperty;
        [SerializeField] private PlayerManager playerManager;

        private void Awake()
        {
            playerInputProperty.playerInputManager = this;

            TryGetComponent(out UnityEngine.InputSystem.PlayerInputManager playerInputManager);

            // InputManagerのイベント追加
            var onPlayerJoined = Observable.FromEvent<PlayerInput>(
                x => playerInputManager.onPlayerJoined += x,
                x => playerInputManager.onPlayerJoined -= x);

            onPlayerJoined.Subscribe(x =>
            {
                if (!x.user.valid)
                    return;

                var deviceid = playerInputProperty.DeviceIds[x.playerIndex];
                x.user.UnpairDevices();

                // デバイスを設定品場合何もしない
                if (deviceid == PlayerInputProperty.UNSETID)
                {
                    return;
                }

                //デバイスの選定
                var devices = InputSystem.devices;
                InputDevice inputDevice = null;
                if (deviceid == PlayerInputProperty.AUTOSETID)
                {
                    inputDevice = AutoSearchInputDevice(x.playerIndex, devices);
                }
                else if (deviceid != PlayerInputProperty.UNSETID)
                {
                    inputDevice = SelectSearchInputDevice(deviceid, devices);
                }
                else
                {
                    return;
                }

                //デバイスセット
                SettingDevice(x, inputDevice);

                //デバイスIDを変更
                playerInputProperty.SettingPlayerDevice(x.playerIndex, inputDevice, false, false);
                playerInputProperty.PlayerInputs[x.playerIndex] = x;
            }).AddTo(this);

            //Input生成
            CreatePlayerInput();
        }

        void CreatePlayerInput()
        {
            var playerObjects = playerManager.PlayerObjects;
            for (int i = 0; i < playerObjects.Length; i++)
            {
                var deviceId = playerInputProperty.DeviceIds[i];
                var playerInput = PlayerInput.Instantiate(inputPrefab, playerIndex: i,
                    pairWithDevice: Keyboard.current);
                playerInput.user.UnpairDevices();

                StartInputObjectSetting(playerInput, transform.parent, i);
            }
        }

        static InputDevice SelectSearchInputDevice(int deviceId, in ReadOnlyArray<InputDevice> devices)
        {
            return devices.FirstOrDefault(device => device.deviceId == deviceId);
        }

        private InputDevice AutoSearchInputDevice(int playerIndex, in ReadOnlyArray<InputDevice> devices)
        {
            foreach (var device in devices)
            {
                if(Mouse.current != null && Mouse.current.deviceId == device.deviceId)
                    continue;

                if (playerInputProperty.DeviceIds.Where((t, i) =>
                        i != playerIndex && device != null && t == device.deviceId).Any())
                {
                    continue;
                }

                return device;
            }

            return null;
        }

        void SettingDevice(PlayerInput playerInput, InputDevice device)
        {
            if (device == null)
                return;

            if ((Keyboard.current != null && Keyboard.current.deviceId == device.deviceId) ||
                Mouse.current != null && Mouse.current.deviceId == device.deviceId)
            {
                playerInput.SwitchCurrentControlScheme("keyboard&Mouse",
                    new InputDevice[] {Keyboard.current, Mouse.current});
            }else if (Gamepad.all.Any(gamepad => gamepad.deviceId == device.deviceId))
            {
                playerInput.SwitchCurrentControlScheme("Gamepad", device);
            }
        }

        void StartInputObjectSetting(PlayerInput playerInput, Transform parent, int playerIndex)
        {
            playerInput.transform.parent = parent;
            playerInput.gameObject.GetComponent<Input>().inputSender = playerInputProperty.inputSenders[playerIndex];
            playerInputProperty.PlayerInputs[playerIndex] = playerInput;
        }

        private void OnDestroy()
        {
            for (int i = 0; i < PlayerInputProperty.MAX_DEVICEID; i++)
            {
                playerInputProperty.SettingPlayerDevice(i, null, false);
            }

            playerInputProperty.playerInputManager = null;
        }
    }
}

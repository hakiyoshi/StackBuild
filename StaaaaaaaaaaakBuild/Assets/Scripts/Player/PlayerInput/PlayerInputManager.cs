using System;
using System.Linq;
using StackBuild.Game;
using UniRx;
using Unity.Netcode;
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

        [field: SerializeField] public int[] CurrentPlayerDevice { get; private set; } =
            new int[PlayerInputProperty.MAX_DEVICEID];

        private void Awake()
        {
            playerInputProperty.playerInputManager = this;

            //デバイスIDコピー
            for (var i = 0; i < playerInputProperty.DeviceIds.Length; i++)
            {
                CurrentPlayerDevice[i] = playerInputProperty.DeviceIds[i];
            }

            // InputManagerのイベント追加
            TryGetComponent(out UnityEngine.InputSystem.PlayerInputManager playerInputManager);
            var onPlayerJoined = Observable.FromEvent<PlayerInput>(
                x => playerInputManager.onPlayerJoined += x,
                x => playerInputManager.onPlayerJoined -= x);

            onPlayerJoined.Subscribe(x =>
            {
                if (!x.user.valid)
                    return;

                var deviceid = CurrentPlayerDevice[x.playerIndex];
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

                //デバイスセット
                SettingDevice(x, inputDevice);

                //デバイスIDを変更
                SettingPlayerDevice(x.playerIndex, inputDevice);
            }).AddTo(this);

            //Input生成
            CreatePlayerInput();
        }

        void CreatePlayerInput()
        {
            var playerObjects = playerManager.PlayerObjects;
            for (int i = 0; i < playerObjects.Length; i++)
            {
                var playerInput = PlayerInput.Instantiate(inputPrefab, playerIndex: i,
                    pairWithDevice: Keyboard.current);
                playerInputProperty.PlayerInputs[i] = playerInput.gameObject;
                playerInput.user.UnpairDevices();
                playerInput.gameObject.SetActive(false);

                StartInputObjectSetting(playerInput, transform.parent, i);
            }
        }

        static InputDevice SelectSearchInputDevice(int deviceId, in ReadOnlyArray<InputDevice> devices)
        {
            return devices.FirstOrDefault(device => device.deviceId == deviceId);
        }

        private InputDevice AutoSearchInputDevice(int playerIndex, in ReadOnlyArray<InputDevice> devices)
        {
            var gamepads = Gamepad.all;
            foreach (var device in devices)
            {
                //マウス
                if((Mouse.current != null && Mouse.current.deviceId == device.deviceId) &&
                   gamepads.All(x => x.deviceId != device.deviceId))
                    continue;

                if (CurrentPlayerDevice.Where((t, i) =>
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
            playerInputProperty.PlayerInputs[playerIndex] = playerInput.gameObject;
        }

        public void SettingPlayerDevice(int playerIndex, InputDevice inputDevice)
        {
            if(inputDevice == null)
                SettingPlayerDevice(playerIndex, PlayerInputProperty.UNSETID);
            else
                SettingPlayerDevice(playerIndex, inputDevice.deviceId);
        }

        public void SettingPlayerDevice(int playerIndex, int deviceId)
        {
            CurrentPlayerDevice[playerIndex] = deviceId;
        }

        private void OnDestroy()
        {
            playerInputProperty.playerInputManager = null;
        }
    }
}

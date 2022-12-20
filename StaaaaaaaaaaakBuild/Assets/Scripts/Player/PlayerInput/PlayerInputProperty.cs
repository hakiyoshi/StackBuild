using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace StackBuild
{
    [CreateAssetMenu(menuName = "Input/PlayerInputProperty")]
    public class PlayerInputProperty : ScriptableObject
    {
        public const int MAX_DEVICEID = 2;
        public const int UNSETID = -1;
        public const int AUTOSETID = -2;

        private int[] deviceIds = new int[]{ AUTOSETID, AUTOSETID };
        public int[] DeviceIds => deviceIds;

        private PlayerInput[] playerInputs = new PlayerInput[MAX_DEVICEID];
        public PlayerInput[] PlayerInputs => playerInputs;

        [field: SerializeField] public InputSender[] inputSenders { get; private set; } = new InputSender[MAX_DEVICEID];

        //自動割り当てを使う場合はautoSetをtrueにしてdeviceをnullにする
        public void SettingPlayerDevice(int playerId, InputDevice device, bool isOnline, bool autoSet = true)
        {
            var id = 0;
            if (!isOnline)
                id = playerId;

            if (id >= MAX_DEVICEID)
                return;

            if (device == null)
            {
                if (!autoSet)
                    deviceIds[id] = UNSETID;
                else
                    deviceIds[id] = AUTOSETID;
            }
            else
            {
                deviceIds[id] = device.deviceId;
            }
        }
    }
}
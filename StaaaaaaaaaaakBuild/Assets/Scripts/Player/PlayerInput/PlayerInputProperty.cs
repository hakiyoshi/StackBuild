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

        private int[] deviceIds = new int[]{ -1, -1 };
        public int[] DeviceIds => deviceIds;

        private PlayerInput[] playerInputs = new PlayerInput[MAX_DEVICEID];
        public PlayerInput[] PlayerInputs => playerInputs;

        public void SettingPlayerDevice(int playerId, InputDevice device, bool isOnline)
        {
            var id = 0;
            if (!isOnline)
                id = playerId;

            if (id >= MAX_DEVICEID)
                return;

            if (device == null)
                deviceIds[id] = UNSETID;
            else
                deviceIds[id] = device.deviceId;
        }


    }
}
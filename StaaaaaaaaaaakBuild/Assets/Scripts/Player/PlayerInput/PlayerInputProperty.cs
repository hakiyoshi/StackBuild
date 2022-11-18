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
        public const int INVALID_ID = -1;

        [field: SerializeField, ReadOnly] public int[] DeviceIds { get; private set; } =
            Enumerable.Repeat<int>(MAX_DEVICEID, INVALID_ID).ToArray();

        public void SettingPlayerDevice(int playerId, KeyControl keyControl)
        {
            SettingPlayerDevice(playerId, keyControl.device);
        }

        public void SettingPlayerDevice(int playerId, ButtonControl buttonControl)
        {
            SettingPlayerDevice(playerId, buttonControl.device);
        }

        public void SettingPlayerDevice(int playerId, InputDevice device)
        {
            if (playerId >= MAX_DEVICEID)
                return;

            DeviceIds[playerId] = device.deviceId;
        }
    }
}
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

        public void SettingPlayerDevice(int playerId, AxisControl inputButton)
        {
            if (playerId >= MAX_DEVICEID)
                return;

            if (inputButton == null)
                DeviceIds[playerId] = INVALID_ID;
            else
                DeviceIds[playerId] = inputButton.device.deviceId;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace NetworkSystem
{
    [CreateAssetMenu(menuName = "Network/Lobby/PlayerOption")]
    public class PlayerOption : ScriptableObject
    {

        [Serializable]
        public struct PlayerOptionData
        {
            public string KeyName;
            public string Value;
            public PlayerDataObject.VisibilityOptions Visibility;
        }

        public List<PlayerOptionData> options;
        
        public void UpdatePlayerOptionValue(string keyName, string Value)
        {
            var index = options.FindIndex(x => x.KeyName == keyName);
            var option = options[index];
            option.Value = Value;
            options[index] = option;
        }

        public Dictionary<string, PlayerDataObject> GetPlayerOptions()
        {
            return options.ToDictionary(
                option => option.KeyName, 
                option => new PlayerDataObject(visibility: option.Visibility, value: option.Value)
                );
        }
    }
}

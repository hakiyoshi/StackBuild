using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace NetworkSystem
{
    [CreateAssetMenu(menuName = "Network/Lobby/LobbyOption")]
    public class LobbyOption : ScriptableObject
    {
        public static string KeyNameRelayJoinCode = "RelayJoinCode";
        public static DataObject.VisibilityOptions VisibilityRelayJoinCode = DataObject.VisibilityOptions.Member;
        public static DataObject.IndexOptions IndexRelayJoinCode = DataObject.IndexOptions.S5;

        [Serializable]
        public struct LobbyOptionData
        {
            public string KeyName;
            public string Value;
            public DataObject.VisibilityOptions Visibility;
            public DataObject.IndexOptions Index;
        }
        
        public List<LobbyOptionData> options;

        private void Reset()
        {
            options = new List<LobbyOptionData>
            {
                new LobbyOptionData()
                {
                    KeyName = KeyNameRelayJoinCode,
                    Value = "ロビー作成時に自動設定される",
                    Visibility = VisibilityRelayJoinCode,
                    Index = IndexRelayJoinCode
                }
            };
        }

        private void OnValidate()
        {
            var index = options.FindIndex(x => x.KeyName == KeyNameRelayJoinCode);
            if (index == -1)
            {
                //ない場合先頭に追加
                options.Insert(0, new LobbyOptionData()
                {
                    KeyName = KeyNameRelayJoinCode,
                    Value = "ロビー作成時に自動設定される",
                    Visibility = VisibilityRelayJoinCode,
                    Index = IndexRelayJoinCode
                });
            }
            else
            {
                options[0] = new LobbyOptionData()
                {
                    KeyName = KeyNameRelayJoinCode,
                    Value = "ロビー作成時に自動設定される",
                    Visibility = VisibilityRelayJoinCode,
                    Index = IndexRelayJoinCode
                };
            }
        }

        public void UpdateLobbyOptionValue(string keyName, string value)
        {
            var index = options.FindIndex(x => x.KeyName == keyName);
            var option = options[index];
            option.Value = value;
            options[index] = option;
        }

        public void UpdateRelayJoinCode(string joinCode)
        {
            UpdateLobbyOptionValue(KeyNameRelayJoinCode, joinCode);
        }

        public Dictionary<string, DataObject> GetPlayerOptions()
        {
            var optionDictionary = options.ToDictionary(
                option => option.KeyName, 
                option => new DataObject(visibility: option.Visibility, value: option.Value, index: option.Index));
            
            return optionDictionary;
        }
    }
}

﻿using System;
using Cysharp.Threading.Tasks;
using NetworkSystem;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;

namespace StackBuild.MatchMaking
{
    public sealed class NetworkSceneChanger : NetworkBehaviour, INetworkSceneChanger
    {
#if UNITY_EDITOR
        [SerializeField] private SceneAsset nextSceneAsset;
#endif
        [SerializeField] private string nextSceneName = "";

#if UNITY_EDITOR
        private void OnValidate()
        {
            nextSceneName = nextSceneAsset.name;
        }
#endif

        public bool SceneChange()
        {
            if (IsSpawned && IsServer)
            {
                NetworkSystemSceneManager.LoadScene(nextSceneName);
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public interface INetworkSceneChanger
    {

    }
}
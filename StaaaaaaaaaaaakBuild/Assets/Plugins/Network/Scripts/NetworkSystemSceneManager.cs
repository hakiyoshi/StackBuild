using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace NetworkSystem
{
    public class NetworkSystemSceneManager : NetworkBehaviour
    {
        private string sceneName;
        private Scene loadedScene;

        public bool SceneIsLoaded
        {
            get { return loadedScene.IsValid() && loadedScene.isLoaded; }
        }

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                NetworkManager.SceneManager.OnSceneEvent += SceneManager_OnSceneEvent;
            }
        }
        public override void OnNetworkDespawn()
        {
            if (IsServer)
            {
                NetworkManager.SceneManager.OnSceneEvent -= SceneManager_OnSceneEvent;
            }
        }

        public static void LoadScene(string sceneName, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            if (!Unity.Netcode.NetworkManager.Singleton.IsServer)
                return;

            var status = Unity.Netcode.NetworkManager.Singleton.SceneManager.LoadScene(sceneName, loadSceneMode);
            CheckStatus(status, sceneName);
        }

        public static void UnloadScene(Scene scene)
        {
            var status = Unity.Netcode.NetworkManager.Singleton.SceneManager.UnloadScene(scene);
            CheckStatus(status, scene.name, false);
        }
        
        public void LoadScene()
        {
            if (!IsServer && !IsSpawned && !loadedScene.isLoaded)
                return;

            NetworkManager.SceneManager.UnloadScene(loadedScene);
        }

        private static void CheckStatus(SceneEventProgressStatus status, string sceneName, bool isLoading = true)
        {
            var sceneEventAction = isLoading ? "load" : "unload";
            if (status != SceneEventProgressStatus.Started)
            {
                Debug.LogWarning($"Failed to load {sceneEventAction} {sceneName}" +
                                 $"with a {nameof(SceneEventProgressStatus)}: {status}");
            }
        }

        private void SceneManager_OnSceneEvent(SceneEvent sceneEvent)
        {
            var clientOrServer = sceneEvent.ClientId == NetworkManager.ServerClientId ? "server" : "client";
            switch (sceneEvent.SceneEventType)
            {
                case SceneEventType.LoadComplete:
                {
                    //サーバーの場合
                    if (sceneEvent.ClientId == NetworkManager.ServerClientId)
                    {
                        //ロードされたシーンを記録する
                        loadedScene = sceneEvent.Scene;
                    }

                    Debug.Log($"Loaded the {sceneEvent.SceneName} scene on {clientOrServer}-({sceneEvent.ClientId}).");
                    break;
                }
                case SceneEventType.UnloadComplete:
                {
                    Debug.Log(
                        $"Unloaded the {sceneEvent.SceneName} scene on {clientOrServer}-({sceneEvent.ClientId}).");
                    break;
                }
                case SceneEventType.LoadEventCompleted:
                case SceneEventType.UnloadEventCompleted:
                {
                    var loadUnload = sceneEvent.SceneEventType == SceneEventType.LoadEventCompleted ? "load" : "unload";
                    Debug.Log($"{loadUnload} event completed for the following " +
                              $"client identifiers:({sceneEvent.ClientsThatCompleted})");
                    if (sceneEvent.ClientsThatTimedOut.Count > 0)
                    {
                        Debug.LogWarning($"{loadUnload} event timed out for the following client " +
                                         $"identifiers:({sceneEvent.ClientsThatTimedOut})");
                    }

                    break;
                }
            }
        }

        
    }
}

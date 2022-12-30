using System;
using NetworkSystem;
using StackBuild.Game;
using UniRx;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace StackBuild.UI.Network
{
    public class ResultButtonNetwork : SyncWaitingSystem, ISelectHandler, IDeselectHandler
    {
#if UNITY_EDITOR
        [SerializeField] private SceneAsset sceneAsset;
        private void OnValidate()
        {
            if (sceneAsset != null)
                sceneName = sceneAsset.name;
        }
#endif
        [SerializeField] private string sceneName = "";
        [SerializeField] private UIInputSender uiInputSender;
        [SerializeField] private float clickCooltime = 0.1f;//連打対策

        private bool standby = false;
        private bool isSelected = false;

        private void Start()
        {
            uiInputSender.Select.sender.Skip(1).ThrottleFirst(TimeSpan.FromSeconds(clickCooltime)).Where(x => x).Subscribe(x =>
            {
                if (!isSelected)
                    return;

                //現在の状態で変える
                if (!standby)
                    ButtonDown();
                else
                    ButtonUp();

                standby = !standby;
            }).AddTo(this);
        }

        protected override void OnSendStandby(int numWaitingToSignal)
        {
            if (IsSpawned && !IsServer)
                return;

            if (numWaitingToSignal >= NetworkManager.ConnectedClientsIds.Count)
            {
                NetworkSystemSceneManager.LoadScene(sceneName);
            }
        }

        public void OnSelect(BaseEventData eventData)
        {
            isSelected = true;
        }

        public void OnDeselect(BaseEventData eventData)
        {
            isSelected = false;
        }

        void ButtonDown()
        {
            //ネットワーク処理
            if(IsSpawned)
            {
                SendStandbyServerRpc();
            }
            else
            {
                //シーンロード処理
                if (sceneName == "")
                    return;

                SceneManager.LoadScene(sceneName);
            }
        }

        void ButtonUp()
        {
            if (IsSpawned)
                SendStandbyReleaseServerRpc();
        }
    }
}
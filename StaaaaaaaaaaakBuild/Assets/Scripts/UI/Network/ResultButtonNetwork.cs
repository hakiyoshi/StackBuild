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

        private bool isPressed = false;
        private bool isSelected = false;

        private void Start()
        {
            uiInputSender.Select.sender.Skip(1).ThrottleFirst(TimeSpan.FromSeconds(clickCooltime)).Where(x => x).Subscribe(x =>
            {
                //選択されてたらボタン押す
                if (isSelected && !isPressed)
                    ButtonDown();

            }).AddTo(this);

            uiInputSender.Cancel.sender.Skip(1).ThrottleFirst(TimeSpan.FromSeconds(clickCooltime)).Where(x => x).Subscribe(x =>
            {
                //選択されてたらボタン放す
                if(isSelected && isPressed)
                    ButtonUp();

            }).AddTo(this);
        }

        protected override void OnSendStandby(int numWaitingToSignal)
        {
            if (IsSpawned && !IsServer)
                return;

            //待機人数が参加人数の場合シーン変更
            if (numWaitingToSignal >= NetworkManager.ConnectedClientsIds.Count)
            {
                SceneLoad(sceneName);
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
                SceneLoad(sceneName);
            }

            isPressed = true;
        }

        void ButtonUp()
        {
            if (IsSpawned)
                SendStandbyReleaseServerRpc();

            isPressed = false;
        }

        void SceneLoad(string name)
        {
            if (name == "")
                return;

            if (IsSpawned)
            {
                NetworkSystemSceneManager.LoadScene(sceneName);
            }
            else
            {
                SceneManager.LoadScene(sceneName);
            }
        }
    }
}
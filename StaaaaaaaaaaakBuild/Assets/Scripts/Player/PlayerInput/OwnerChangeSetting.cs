using StackBuild.Game;
using UniRx;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace StackBuild
{
    public class OwnerChangeSetting : NetworkBehaviour
    {
        [SerializeField] private PlayerInputProperty playerInputProperty;
        [SerializeField] private PlayerManager playerManager;
        [SerializeField] private MatchControlState matchControlState;

        private int playerIndex
        {
            get
            {
                return playerManager.GetPlayerIndex(this.gameObject);
            }
        }

        GameObject GetInput()
        {
            return playerInputProperty.PlayerInputs[playerIndex];
        }

        private void Start()
        {
            matchControlState.State.Skip(1).Subscribe(x =>
            {
                if (x == MatchState.Ingame)
                {
                    var input = GetInput();

                    if (IsSpawned)
                    {
                        if (IsOwner)
                        {
                            GainedInput(input);
                        }
                        else
                        {
                            LostInput(input);
                        }
                        return;
                    }

                    input.gameObject.SetActive(playerInputProperty.playerInputManager.CurrentPlayerDevice[playerIndex] !=
                                               PlayerInputProperty.UNSETID);
                } else if (x == MatchState.Finished)
                {
                    var input = GetInput();
                    LostInput(input);
                }
            }).AddTo(this);
        }

        public override void OnNetworkDespawn()
        {
            var playerInput = GetInput();
            if(playerInput == null || playerInputProperty.playerInputManager == null)
                return;

            playerInputProperty.playerInputManager.SettingPlayerDevice(playerIndex,
                playerInputProperty.DeviceIds[playerIndex]);
            playerInput.gameObject.SetActive(true);
        }

        public override void OnGainedOwnership()
        {
            var playerInput = GetInput();
            if(playerInput == null)
                return;

            GainedInput(playerInput);
        }

        public override void OnLostOwnership()
        {
            var playerInput = GetInput();
            if(playerInput == null)
                return;

            LostInput(playerInput);
        }

        void LostInput(GameObject inputObject)
        {
            inputObject.SetActive(false);
        }

        void GainedInput(GameObject inputObject)
        {
            SwitchDevice();
            inputObject.gameObject.SetActive(true);
        }

        void SwitchDevice()
        {
            //１Pの設定を持ってくる
            var playerInputManager = playerInputProperty.playerInputManager;
            playerInputManager.SettingPlayerDevice(playerIndex, playerInputProperty.playerInputManager.CurrentPlayerDevice[0]);
        }
    }
}
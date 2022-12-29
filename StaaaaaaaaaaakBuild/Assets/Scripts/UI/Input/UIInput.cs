using System;
using StackBuild.Game;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

namespace StackBuild.UI
{
    public class UIInput : MonoBehaviour
    {
        [SerializeField] private UIInputSender uiInputSender;
        private PlayerInput playerInput;

        private void Start()
        {
            TryGetComponent(out playerInput);

            var ui = playerInput.actions.FindActionMap("UI");
            InputFromEvent.ActionMapFromEventStarted(ui, "Select").Subscribe(_ => { uiInputSender.Select.Send(true); }).AddTo(this);
            InputFromEvent.ActionMapFromEventCanceled(ui, "Select").Subscribe(_ => { uiInputSender.Select.Send(false); }).AddTo(this);

            InputFromEvent.ActionMapFromEventStarted(ui, "Cancel").Subscribe(_ => { uiInputSender.Cancel.Send(true); }).AddTo(this);
            InputFromEvent.ActionMapFromEventCanceled(ui, "Cancel").Subscribe(_ => { uiInputSender.Cancel.Send(false); }).AddTo(this);
        }
    }
}
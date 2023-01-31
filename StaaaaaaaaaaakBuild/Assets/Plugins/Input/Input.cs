using System;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class Input : MonoBehaviour
{
    public InputSender inputSender;

    private PlayerInput input;

    private void Start()
    {
        TryGetComponent(out input);

        var player = input.actions.FindActionMap("Player");
        InputFromEvent.ActionMapFromEventPerformed(player, "Move").Subscribe(x => inputSender.Move.Send(x.ReadValue<Vector2>())).AddTo(this);
        InputFromEvent.ActionMapFromEventCanceled(player, "Move").Subscribe(_ => inputSender.Move.Send(Vector2.zero)).AddTo(this);

        InputFromEvent.ActionMapFromEventStarted(player, "Catch").Subscribe(_ => inputSender.Catch.Send(true)).AddTo(this);
        InputFromEvent.ActionMapFromEventCanceled(player, "Catch").Subscribe(_ => inputSender.Catch.Send(false)).AddTo(this);

        InputFromEvent.ActionMapFromEventStarted(player, "Dash").Subscribe(_ => inputSender.Dash.Send(true)).AddTo(this);
        InputFromEvent.ActionMapFromEventCanceled(player, "Dash").Subscribe(_ => inputSender.Dash.Send(false)).AddTo(this);
    }

    private void OnEnable()
    {
        inputSender.Catch.isPause = false;
        inputSender.Dash.isPause = false;
        inputSender.Move.isPause = false;
    }

    private void OnDisable()
    {
        // inputSender.Catch.isPause = true;
        // inputSender.Dash.isPause = true;
        // inputSender.Move.isPause = true;
    }
}

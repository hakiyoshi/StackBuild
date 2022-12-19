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
        ActionMapFromEventPerformed(player, "Move").Subscribe(x => inputSender.Move.Send(x.ReadValue<Vector2>())).AddTo(this);
        ActionMapFromEventCanceled(player, "Move").Subscribe(_ => inputSender.Move.Send(Vector2.zero)).AddTo(this);

        ActionMapFromEventStarted(player, "Catch").Subscribe(_ => inputSender.Catch.Send(true)).AddTo(this);
        ActionMapFromEventCanceled(player, "Catch").Subscribe(_ => inputSender.Catch.Send(false)).AddTo(this);

        ActionMapFromEventStarted(player, "Dash").Subscribe(_ => inputSender.Dash.Send(true)).AddTo(this);
        ActionMapFromEventCanceled(player, "Dash").Subscribe(_ => inputSender.Dash.Send(false)).AddTo(this);
    }

    private IObservable<InputAction.CallbackContext> ActionMapFromEventStarted(InputActionMap actionMap, string actionName)
    {
        return Observable.FromEvent<InputAction.CallbackContext>(
            x => actionMap[actionName].started += x,
            x => actionMap[actionName].started -= x);
    }

    private IObservable<InputAction.CallbackContext> ActionMapFromEventPerformed(InputActionMap actionMap, string actionName)
    {
        return Observable.FromEvent<InputAction.CallbackContext>(
            x => actionMap[actionName].performed += x,
            x => actionMap[actionName].performed -= x);
    }

    private IObservable<InputAction.CallbackContext> ActionMapFromEventCanceled(InputActionMap actionMap, string actionName)
    {
        return Observable.FromEvent<InputAction.CallbackContext>(
            x => actionMap[actionName].canceled += x,
            x => actionMap[actionName].canceled -= x);
    }

    private void OnEnable()
    {
        inputSender.Catch.isPause = true;
        inputSender.Dash.isPause = true;
        inputSender.Move.isPause = true;
    }

    private void OnDisable()
    {
        inputSender.Catch.isPause = false;
        inputSender.Dash.isPause = false;
        inputSender.Move.isPause = false;
    }
}

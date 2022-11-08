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
        ActionMapFromEventStarted(player, "Catch").Subscribe(x => inputSender.SendCatch(true)).AddTo(this);
        ActionMapFromEventCanceled(player, "Catch").Subscribe(x => inputSender.SendCatch(false)).AddTo(this);
        
        ActionMapFromEventPerformed(player, "Move").Subscribe(x => inputSender.SendMove(x.ReadValue<Vector2>())).AddTo(this);
        ActionMapFromEventCanceled(player, "Move").Subscribe(x => inputSender.SendMove(Vector2.zero)).AddTo(this);
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
}

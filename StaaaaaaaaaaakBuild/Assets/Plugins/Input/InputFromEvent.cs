﻿using System;
using UniRx;
using UnityEngine.InputSystem;

public class InputFromEvent
{
    public static IObservable<InputAction.CallbackContext> ActionMapFromEventStarted(InputActionMap actionMap, string actionName)
    {
        return Observable.FromEvent<InputAction.CallbackContext>(
            x => actionMap[actionName].started += x,
            x => actionMap[actionName].started -= x);
    }

    public static IObservable<InputAction.CallbackContext> ActionMapFromEventPerformed(InputActionMap actionMap, string actionName)
    {
        return Observable.FromEvent<InputAction.CallbackContext>(
            x => actionMap[actionName].performed += x,
            x => actionMap[actionName].performed -= x);
    }

    public static IObservable<InputAction.CallbackContext> ActionMapFromEventCanceled(InputActionMap actionMap, string actionName)
    {
        return Observable.FromEvent<InputAction.CallbackContext>(
            x => actionMap[actionName].canceled += x,
            x => actionMap[actionName].canceled -= x);
    }
}
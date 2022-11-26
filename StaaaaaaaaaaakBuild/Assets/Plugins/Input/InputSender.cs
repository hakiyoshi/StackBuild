using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName = "InputSender")]
public class InputSender : ScriptableObject
{
    public IReadOnlyReactiveProperty<Vector2> Move => move;
    private ReactiveProperty<Vector2> move = new ReactiveProperty<Vector2>();

    public IReadOnlyReactiveProperty<bool> Catch => catchHold;
    private ReactiveProperty<bool> catchHold = new ReactiveProperty<bool>();

    public IReadOnlyReactiveProperty<bool> Dash => dash;
    private ReactiveProperty<bool> dash = new ReactiveProperty<bool>();

    public void SendMove(Vector2 value)
    {
        move.Value = value;
    }

    public void SendCatch(bool value)
    {
        catchHold.Value = value;
    }

    public void SendDash(bool value)
    {
        dash.Value = value;
    }
}

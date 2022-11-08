using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

[CreateAssetMenu(menuName = "InputSender")]
public class InputSender : ScriptableObject
{

    public IReadOnlyReactiveProperty<Vector2> Move => move; 
    public IReadOnlyReactiveProperty<bool> Catch => catchHold;

    private ReactiveProperty<Vector2> move = new ReactiveProperty<Vector2>();
    private ReactiveProperty<bool> catchHold = new ReactiveProperty<bool>();

    public void SendMove(Vector2 value)
    {
        move.Value = value;
    }

    public void SendCatch(bool value)
    {
        catchHold.Value = value;
    }
}

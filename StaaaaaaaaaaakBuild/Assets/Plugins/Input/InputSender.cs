using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName = "InputSender")]
public class InputSender : ScriptableObject
{
    public SenderProperty<Vector2> Move = new SenderProperty<Vector2>(Vector2.zero);
    public SenderProperty<bool> Catch = new SenderProperty<bool>(false);
    public SenderProperty<bool> Dash = new SenderProperty<bool>(false);

    public void AllSetIsPause(bool pause) {
        Move.isPause = pause;
        Catch.isPause = pause;
        Dash.isPause = pause;
    }
}

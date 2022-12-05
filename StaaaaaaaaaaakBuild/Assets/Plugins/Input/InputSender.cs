using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName = "InputSender")]
public class InputSender : ScriptableObject
{
    public class SenderProperty<T>
    {
        public IReadOnlyReactiveProperty<T> sender => data;
        private ReactiveProperty<T> data = new ReactiveProperty<T>();
        public bool isPause = false;

        public T Value => data.Value;

        public void Send(T value)
        {
            if (isPause)
                return;

            data.Value = value;
        }
    }

    public SenderProperty<Vector2> Move = new SenderProperty<Vector2>();
    public SenderProperty<bool> Catch = new SenderProperty<bool>();
    public SenderProperty<bool> Dash = new SenderProperty<bool>();
}

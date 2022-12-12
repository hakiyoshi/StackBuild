using System.ComponentModel;
using UnityEngine;
using UnityEngine.InputSystem;

namespace StackBuild
{
    public class CameraDebug : MonoBehaviour
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        [Header("PキーでGUI表示非表示")]
        [SerializeField] private bool isDrawGUI = true;
        [SerializeField] private bool isDebug = false;

        private Vector3 initialPosition;
        private Quaternion initialRotation;
        private Vector3 initialScale;

        private Rect windowRect = new Rect(0f, 0f, 200f, 200f);
        private bool isDebugBefore = false;

        //GUILayoutで変更ができるクラス
        [System.Serializable]
        class LayoutValue<T>
        {
            public T value;
            private string cashText;

            public void DrawGUI(string label)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(label);
                cashText = GUILayout.TextField(cashText);
                if (TryParse(cashText, out T result))
                    value = result;
                GUILayout.EndHorizontal();
            }

            bool TryParse(string input, out T result)
            {
                try
                {
                    var converter = TypeDescriptor.GetConverter(typeof(T));
                    if (converter != null)
                    {
                        result = (T) converter.ConvertFromString(input);
                        return true;
                    }

                    result = default(T);
                    return false;
                }
                catch
                {
                    result = default(T);
                    return false;
                }
            }

            public LayoutValue(T a = default(T))
            {
                value = a;
                cashText = value.ToString();
            }

            public static implicit operator T(LayoutValue<T> value)
            {
                return value.value;
            }
        }

        [SerializeField] private LayoutValue<float> moveSpeed = new LayoutValue<float>(50.0f);
        [SerializeField] private LayoutValue<float> mouseSensitivity = new LayoutValue<float>(0.1f);

        private Vector2 rotationClickPoint = Vector2.zero;
        private Vector2 moveClickPoint = Vector2.zero;

        private void Update()
        {
            if (Keyboard.current.pKey.wasPressedThisFrame)
                isDrawGUI = !isDrawGUI;

            if (!isDebug)
                return;

            MoveDebug();
            RotationDebug();
        }

        void MoveDebug()
        {
            var velocity = Vector3.zero;

            //キーボード
            if (Keyboard.current.wKey.isPressed)
                velocity += new Vector3(transform.forward.x, 0.0f, transform.forward.z);

            if (Keyboard.current.aKey.isPressed)
                velocity += new Vector3(-transform.right.x, 0.0f, -transform.right.z);

            if (Keyboard.current.sKey.isPressed)
                velocity += new Vector3(-transform.forward.x, 0.0f, -transform.forward.z);

            if (Keyboard.current.dKey.isPressed)
                velocity += new Vector3(transform.right.x, 0.0f, transform.right.z);

            velocity.Normalize();

            //マウス中ボダン移動
            if (Mouse.current.middleButton.isPressed)
            {
                var mousePosi = Mouse.current.position.ReadValue();

                if (Mouse.current.middleButton.wasPressedThisFrame)
                    moveClickPoint = mousePosi;

                var acc = mousePosi - moveClickPoint;
                velocity += transform.up * -acc.y;
                velocity += transform.right * -acc.x;

                moveClickPoint = mousePosi;

                //マウス移動
                WarpMouse();
            }

            //マウス中ボタンくｒくｒ
            var scroll = Mouse.current.scroll.ReadValue();
            if (scroll.y > 0)
            {
                velocity += transform.forward * 10.0f;
            }
            else if (scroll.y < 0)
            {
                velocity += -transform.forward * 10.0f;
            }

            transform.position += velocity * (moveSpeed * Time.deltaTime);
        }

        void RotationDebug()
        {
            Quaternion rotax = Quaternion.identity;
            Quaternion rotay = Quaternion.identity;
            var mousePosi = Mouse.current.position.ReadValue();

            //入力があったら
            if (Mouse.current.rightButton.wasPressedThisFrame ||
                Mouse.current.leftButton.wasPressedThisFrame)
                rotationClickPoint = mousePosi;

            var acc = mousePosi - rotationClickPoint;

            if (Mouse.current.rightButton.isPressed)//自分回転
            {
                //回転
                rotax *= Quaternion.AngleAxis(-acc.y * mouseSensitivity, Vector3.right);
                rotay *= Quaternion.AngleAxis(acc.x * mouseSensitivity, Vector3.up);

                rotationClickPoint = Mouse.current.position.ReadValue();

                //マウス移動
                WarpMouse();

                transform.rotation = rotay * transform.rotation * rotax;
            }
            else if (Mouse.current.leftButton.isPressed)//注視点回転
            {
                //回転
                rotax *= Quaternion.AngleAxis(acc.y * mouseSensitivity, Vector3.right);
                rotay *= Quaternion.AngleAxis(-acc.x * mouseSensitivity, Vector3.up);

                rotationClickPoint = Mouse.current.position.ReadValue();

                //マウス移動
                WarpMouse();

                transform.position += transform.forward * 50.0f;
                transform.rotation = rotay * transform.rotation * rotax;
                transform.position += -transform.forward * 50.0f;
            }
        }



        void WarpMouse()
        {
            var mousePosi = Mouse.current.position.ReadValue();
            var warp = new Vector2(-1f, -1f);
            if(mousePosi.x >= Screen.width)
            {
                warp = new Vector2(1.0f, mousePosi.y);
            }
            else if(mousePosi.x <= 0.0f)
            {
                warp = new Vector2(Screen.width - 1.0f, mousePosi.y);
            }

            if(mousePosi.y >= Screen.height)
            {
                warp = new Vector2(mousePosi.x, 1.0f);
            }
            else if (mousePosi.y <= 0.0f)
            {
                warp = new Vector2(mousePosi.x, Screen.height - 1.0f);
            }

            if (warp.x >= 0)
            {
                Mouse.current.WarpCursorPosition(warp);
                rotationClickPoint = warp;
                moveClickPoint = warp;
            }
        }

        //-----------------------------------------

        private void OnGUI()
        {
            if (isDrawGUI)
            {
                windowRect = GUILayout.Window(0, windowRect, DebugWindow, "Camera");
            }
        }

        void DebugWindow(int windowid)
        {
            GUILayout.Label("PキーでGUI表示非表示");

            //デバッグ機能のONOFF
            isDebug = GUILayout.Toggle(isDebug, "DebugMode");
            if (isDebug == isDebugBefore)//IsDebugが変更されたら
            {
                isDebugBefore = isDebug;
                SetInitialTransform();
            }

            GUI.enabled = isDebug;

            //リセット
            if (GUILayout.Button("Reset"))
                ResetTransform();

            //移動速度変更
            moveSpeed.DrawGUI("MoveSpeed");

            //マウス感度
            mouseSensitivity.DrawGUI("MouseSensitivity");

            GUI.enabled = true;

            GUI.DragWindow();
        }

        void ResetTransform()
        {
            var transform1 = transform;
            transform1.position = initialPosition;
            transform1.rotation = initialRotation;
            transform1.localScale = initialScale;
        }

        void SetInitialTransform()
        {
            var transform1 = transform;
            initialPosition = transform1.position;
            initialRotation = transform1.rotation;
            initialScale = transform1.localScale;
        }
#endif
    }
}

using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Rendering;

namespace StackBuild
{
    [CustomEditor(typeof(Canon))]
    public class CanonEditor : Editor
    {
        private Canon instance;

        private void OnEnable()
        {
            instance = (Canon)target;

        }

        private void OnSceneGUI()
        {
            var transform = instance.transform;

            EditorGUI.BeginChangeCheck();

            var powerSlider = Handles.Slider(
                transform.position + Vector3.up * instance.ShootPower,
                transform.up);
            if (EditorGUI.EndChangeCheck())
            {
                instance.ShootPower = Vector3.Dot(transform.up, powerSlider);
            }
        }
    }
}
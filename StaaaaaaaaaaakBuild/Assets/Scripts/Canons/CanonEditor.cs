using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Rendering;

namespace StackBuild
{
    [CustomEditor(typeof(CanonCore))]
    public class CanonEditor : Editor
    {
        private CanonCore instance;

        private void OnEnable()
        {
            instance = (CanonCore)target;

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
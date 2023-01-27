using System;
using StackBuild.UI;
using UnityEditor;
using UnityEditor.UI;

namespace StackBuild.Editor
{

    // [SerializeField] が効かなかったのでカスタムエディター
    [CustomEditor(typeof(SkewedImage))]
    [CanEditMultipleObjects]
    public class SkewedImageEditor : ImageEditor
    {

        private SerializedProperty skewFactor;
        private SerializedProperty skewLeft;
        private SerializedProperty skewRight;
        private SerializedProperty shrink;


        protected override void OnEnable()
        {
            base.OnEnable();
            skewFactor = serializedObject.FindProperty("skewFactor");
            skewLeft   = serializedObject.FindProperty("skewLeft");
            skewRight  = serializedObject.FindProperty("skewRight");
            shrink     = serializedObject.FindProperty("shrink");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            EditorGUILayout.PropertyField(skewFactor);
            EditorGUILayout.PropertyField(skewLeft);
            EditorGUILayout.PropertyField(skewRight);
            EditorGUILayout.PropertyField(shrink);
            serializedObject.ApplyModifiedProperties();
        }

    }
}
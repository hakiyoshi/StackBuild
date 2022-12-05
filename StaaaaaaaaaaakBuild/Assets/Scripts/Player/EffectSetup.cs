using System;
using Unity.VisualScripting;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace StackBuild
{
    public class EffectSetup : MonoBehaviour, IPreprocessBuildWithReport
    {
        [SerializeField] private PlayerProperty playerProperty;
        [SerializeField] private MeshRenderer effectMeshRenderer;

        private CharacterProperty property
        {
            get
            {
                return playerProperty.characterProperty;
            }
        }

#if UNITY_EDITOR
        [ContextMenu("SetupEffect")]
        void SetupEffect()
        {
            Setup();
            effectMeshRenderer.transform.localScale = playerProperty.characterProperty.Catch.CatchEffectMaxSizeOffset;
        }
#endif

        private void Awake()
        {
            Setup();
        }

        void Setup()
        {
            if (playerProperty.characterProperty == null)
                return;

            //エフェクト１
            effectMeshRenderer.material = property.Catch.EffectMaterial1;

            //エフェクト２
            effectMeshRenderer.transform.GetChild(0).GetComponent<MeshRenderer>().material =
                property.Catch.EffectMaterial2;
        }

        public int callbackOrder
        {
            get { return 0; }
        }
        public void OnPreprocessBuild(BuildReport report)
        {
            effectMeshRenderer.transform.localScale = Vector3.zero;
        }
    }
}
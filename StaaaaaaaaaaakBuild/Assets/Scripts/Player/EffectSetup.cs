using System;
using Unity.VisualScripting;
using UnityEngine;

namespace StackBuild
{
    public class EffectSetup : MonoBehaviour
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
    }
}
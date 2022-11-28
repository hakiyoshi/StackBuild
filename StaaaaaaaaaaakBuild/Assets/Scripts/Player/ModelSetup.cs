using System;
using UnityEditor;
using UnityEngine;

namespace StackBuild
{
    public class ModelSetup : MonoBehaviour
    {
        [SerializeField] private PlayerProperty playerProperty;

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
            if(playerProperty.characterProperty == null)
                return;

            GameObject.Instantiate(property.CharacterModelPrefab, transform);
        }
    }
}
using System;
using UnityEngine;

namespace StackBuild
{
    public class ModelSetup : MonoBehaviour
    {
        [SerializeField] private PlayerProperty playerProperty;

        private void Awake()
        {
            Setup();
        }

        void Setup()
        {
            if(playerProperty.characterProperty == null)
                return;

            GameObject.Instantiate(playerProperty.characterProperty.CharacterModelPrefab, transform);
        }
    }
}
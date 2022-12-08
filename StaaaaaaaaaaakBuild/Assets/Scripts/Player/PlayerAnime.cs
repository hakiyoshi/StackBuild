using System;
using Unity.VisualScripting;
using UnityEngine;

namespace StackBuild
{
    public class PlayerAnime : MonoBehaviour
    {
        [SerializeField] private ModelSetup modelSetup;

        private Animator animator;

        private void Start()
        {
            modelSetup.modelObject.TryGetComponent(out animator);
        }
    }
}
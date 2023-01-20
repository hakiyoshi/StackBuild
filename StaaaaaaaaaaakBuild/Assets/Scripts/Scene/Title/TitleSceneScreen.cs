using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace StackBuild.Scene.Title
{

    public class TitleSceneScreen : MonoBehaviour
    {

        [SerializeField] private bool shouldShowLogo;

        public bool ShouldShowLogo => shouldShowLogo;

#pragma warning disable CS1998

        public virtual async UniTask ShowAsync()
        {
        }

        public virtual async UniTask HideAsync()
        {
        }

#pragma warning restore

    }

}

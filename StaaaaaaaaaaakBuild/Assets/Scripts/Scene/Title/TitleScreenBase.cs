using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace StackBuild.UI.Scene.Title
{

    public class TitleScreenBase : MonoBehaviour
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

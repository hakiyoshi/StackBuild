using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace StackBuild.Scene.Title
{
    public class MatchmakingScreen : TitleSceneScreen
    {

        [SerializeField] private CanvasGroup container;

        public override async UniTask ShowAsync()
        {
            await container.DOFade(1, 0.3f);
        }

        public override async UniTask HideAsync()
        {
            await container.DOFade(0, 0.1f);
        }

    }
}
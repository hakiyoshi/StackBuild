using Cysharp.Threading.Tasks;
using DG.Tweening;
using StackBuild.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace StackBuild.Scene.Title
{

    public class SettingsScreen : TitleSceneScreen
    {

        [SerializeField] private CanvasGroup container;
        [SerializeField] private TitleMenuStaggerDisplay staggerDisplay;
        [SerializeField] private AudioMixerVolumeSlider[] volumeSliders;
        [SerializeField] private Button buttonBack;

        public Button.ButtonClickedEvent OnBackClick => buttonBack.onClick;

        public override async UniTask ShowAsync()
        {
            foreach (var slider in volumeSliders)
            {
                slider.UpdateSliderValue();
            }

            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(volumeSliders[0].gameObject);

            container.interactable = true;
            container.blocksRaycasts = true;
            container.alpha = 1;

            await staggerDisplay.Display();
        }

        public override async UniTask HideAsync()
        {
            container.interactable = false;
            container.blocksRaycasts = false;
            await container.DOFade(0, 0.07f);
            staggerDisplay.Hide();
        }

    }

}

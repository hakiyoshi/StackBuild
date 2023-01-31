using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using StackBuild.UI;
using TMPro;
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
        [SerializeField] private TMP_Dropdown resolutionDropdown;
        [SerializeField] private Toggle fullscreenCheck;
        [SerializeField] private Button buttonBack;

        private Resolution[] resolutions;

        public Button.ButtonClickedEvent OnBackClick => buttonBack.onClick;

        private void Awake()
        {
            resolutions = Screen.resolutions.Distinct(ResolutionComparer.Instance).ToArray();
            resolutionDropdown.options = resolutions.Select(resolution =>
                new TMP_Dropdown.OptionData($"{resolution.width.ToString()} x {resolution.height.ToString()}")).ToList();
            resolutionDropdown.onValueChanged.AddListener(idx =>
            {
                var resolution = resolutions[idx];
                Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
            });

            fullscreenCheck.onValueChanged.AddListener(value =>
            {
                resolutionDropdown.interactable = !value;
                if (value)
                {
                    Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height,
                            FullScreenMode.FullScreenWindow);
                }
                else
                {
                    var resolution = resolutions[resolutionDropdown.value];
                    Screen.SetResolution(resolution.width, resolution.height, FullScreenMode.Windowed);
                }
            });
        }

        public override async UniTask ShowAsync()
        {
            foreach (var slider in volumeSliders)
            {
                slider.UpdateSliderValue();
            }

            resolutionDropdown.interactable = !Screen.fullScreen;
            for (int i = 0; i < resolutions.Length; i++)
            {
                var resolution = resolutions[i];
                if (resolution.width == Screen.width && resolution.height == Screen.height)
                {
                    resolutionDropdown.value = i;
                    break;
                }
            }

            fullscreenCheck.isOn = Screen.fullScreen;

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

        private class ResolutionComparer : IEqualityComparer<Resolution>
        {

            public static readonly ResolutionComparer Instance = new();

            bool IEqualityComparer<Resolution>.Equals(Resolution x, Resolution y)
            {
                return x.width == y.width && x.height == y.height;
            }

            int IEqualityComparer<Resolution>.GetHashCode(Resolution obj)
            {
                return obj.GetHashCode();
            }
        }

    }

}

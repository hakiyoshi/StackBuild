using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace StackBuild.UI
{
    [RequireComponent(typeof(Slider))]
    public class AudioMixerVolumeSlider : MonoBehaviour
    {

        [SerializeField] private Slider slider;
        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private string mixerParameterName;

        private void Reset()
        {
            slider = GetComponent<Slider>();
        }

        private void Awake()
        {
            slider.onValueChanged.AddListener(ApplyVolume);
            UpdateSliderValue();
        }

        /// このスライダーに、現在のAudioMixerの音量を示させます。
        /// 他の場所で音量を変えると自動更新されないので、
        /// このスライダーを含む画面の表示時にでも呼んでください。
        public void UpdateSliderValue()
        {
            if (!audioMixer.GetFloat(mixerParameterName, out var db))
                throw new ArgumentOutOfRangeException($"Parameter does not exist: {mixerParameterName}");
            slider.value = Mathf.Clamp(Mathf.Pow(10, Mathf.Clamp(db, -80, 0) / 20f), 0, 1);
        }

        private void ApplyVolume(float volume)
        {
            if (!audioMixer.SetFloat(mixerParameterName, Mathf.Clamp(Mathf.Log10(volume) * 20f, -80f, 0f)))
                throw new ArgumentOutOfRangeException($"Parameter does not exist: {mixerParameterName}");
        }

    }
}

using System;
using StackBuild.Audio;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace StackBuild.UI
{
    [RequireComponent(typeof(Selectable))]
    public class UISoundPlayer : MonoBehaviour, ISelectHandler, IPointerEnterHandler
    {

        [SerializeField] private Selectable ui;
        [SerializeField] private AudioChannel channel;
        [SerializeField] private AudioCue cursorSound;
        [SerializeField] private AudioCue selectSound;

        private void Reset()
        {
            ui = GetComponent<Selectable>();
        }

        private void Awake()
        {
            if (ui is Button button)
            {
                button.onClick.AddListener(PlaySelectSound);
            }
        }

        void ISelectHandler.OnSelect(BaseEventData eventData)
        {
            PlayCursorSound();
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            PlayCursorSound();
        }

        private void PlayCursorSound()
        {
            if (!ui.interactable) return;
            channel.Request(cursorSound);
        }

        private void PlaySelectSound()
        {
            if (!ui.interactable) return;
            channel.Request(selectSound);
        }

    }
}

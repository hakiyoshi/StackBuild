using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace StackBuild.UI.Network
{
    public class ResultButton :  MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private string text;
        [SerializeField] private Sprite iconImage;
        [SerializeField] private bool showIcon;
        [SerializeField] private bool disabled;
        [SerializeField] private StackbuildButtonStyle style;
        [Space]
        [SerializeField] private Button button;
        [SerializeField] private TMP_Text label;
        [SerializeField] private Image background;
        [SerializeField] private Image border;
        [SerializeField] private Image icon;

        [Space]
        [SerializeField] private UIInputSender inputSender;
        [SerializeField] private EventSystem eventSystem;


        private bool isSelected;
        private bool isHovered;

        private bool isPress = false;
        private bool IsIsPress
        {
            get { return isPress; }
            set
            {
                isPress = value;
                eventSystem.enabled = !value;
            }
        }

        private void Start()
        {
            inputSender.Select.sender.Skip(1).Where(x => x).Subscribe(x =>
            {
                if(!isSelected)
                    return;

                IsIsPress = true;
                UpdateColor();

            }).AddTo(this);

            inputSender.Cancel.sender.Skip(1).Where(x => x).Subscribe(x =>
            {
                if(!isSelected)
                    return;

                IsIsPress = false;
                UpdateColor();
            }).AddTo(this);
        }

        private void Reset()
        {
            button = GetComponent<Button>();
            button.transition = Selectable.Transition.None;
        }

        private void OnValidate()
        {
            if (button == null || background == null || border == null || label == null) return;
            button.interactable = !disabled;
            label.text = text;
            if (icon != null)
            {
                icon.gameObject.SetActive(showIcon);
                icon.sprite = iconImage;
            }
            UpdateColor();
        }

        private void UpdateColor()
        {
            if(style == null) return;
            if (disabled)
            {
                background.color = style.DisabledBackgroundColor;
                border.color     = style.DisabledBorderColor;
                label.color      = style.DisabledTextColor;
                return;
            }

            bool isSelectedOrHovered = isSelected || isHovered;

            background.color = IsIsPress ? style.PressedBackgroundColor :
                isSelectedOrHovered ? style.HoveredBackgroundColor : style.NormalBackgroundColor;
            border.color     = style.NormalBorderColor;
            label.color      = style.NormalTextColor;
        }

        void ISelectHandler.OnSelect(BaseEventData eventData)
        {
            if (disabled && IsIsPress) return;
            isSelected = true;
            UpdateColor();
        }

        void IDeselectHandler.OnDeselect(BaseEventData eventData)
        {
            if (disabled && IsIsPress) return;
            isSelected = false;
            UpdateColor();
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            if (disabled && IsIsPress) return;
            isHovered = true;
            UpdateColor();
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            if (disabled && IsIsPress) return;
            isHovered = false;
            UpdateColor();
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            if (disabled) return;
            IsIsPress = true;
            UpdateColor();
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            if (disabled) return;
            IsIsPress = false;
            UpdateColor();
        }

    }
}
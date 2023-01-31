using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace StackBuild.UI
{
    [RequireComponent(typeof(Button))]
    public class StackbuildButton : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
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

        private bool isSelected;
        private bool isHovered;
        private bool isPressed;

        public string Text
        {
            set => label.text = text = value;
        }
        public bool Disabled
        {
            get => disabled;
            set
            {
                disabled = value;
                button.interactable = !disabled;
                UpdateColor();
            }
        }

        public Button.ButtonClickedEvent OnClick => button.onClick;

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

            background.color = isPressed ? style.PressedBackgroundColor :
                isSelectedOrHovered ? style.HoveredBackgroundColor : style.NormalBackgroundColor;
            border.color     = style.NormalBorderColor;
            label.color      = style.NormalTextColor;
        }

        void ISelectHandler.OnSelect(BaseEventData eventData)
        {
            if (disabled) return;
            isSelected = true;
            UpdateColor();
        }

        void IDeselectHandler.OnDeselect(BaseEventData eventData)
        {
            if (disabled) return;
            isSelected = false;
            UpdateColor();
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            if (disabled) return;
            isHovered = true;
            UpdateColor();
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            if (disabled) return;
            isHovered = false;
            UpdateColor();
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            if (disabled) return;
            isPressed = true;
            UpdateColor();
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            if (disabled) return;
            isPressed = false;
            UpdateColor();
        }

    }
}

using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace StackBuild.UI
{
    [RequireComponent(typeof(Button))]
    public class StackbuildButton : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {

        private static readonly Color NormalBackgroundColor   = new(1.0f,   1.0f,   1.0f,   0.25f);
        private static readonly Color NormalBorderColor       = new(1.0f,   1.0f,   1.0f,   1.0f);
        private static readonly Color NormalTextColor         = new(0.0f,   0.0f,   0.0f,   1.0f);
        private static readonly Color HoveredBackgroundColor  = new(1.0f,   1.0f,   1.0f,   1.0f);
        private static readonly Color PressedBackgroundColor  = new(0.878f, 0.878f, 0.878f, 1.0f);
        private static readonly Color DisabledBackgroundColor = new(0.502f, 0.502f, 0.502f, 0.5f);
        private static readonly Color DisabledBorderColor     = new(0.502f, 0.502f, 0.502f, 1.0f);
        private static readonly Color DisabledTextColor       = new(0.2f,   0.2f,   0.2f,   1.0f);

        [SerializeField] private string text;
        [SerializeField] private Sprite iconImage;
        [SerializeField] private bool showIcon;
        [SerializeField] private bool disabled;
        [Space]
        [SerializeField] private Button button;
        [SerializeField] private TMP_Text label;
        [SerializeField] private Image background;
        [SerializeField] private Image border;
        [SerializeField] private Image icon;

        private bool isSelected;
        private bool isHovered;
        private bool isPressed;

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
            if (disabled)
            {
                background.color = DisabledBackgroundColor;
                border.color     = DisabledBorderColor;
                label.color      = DisabledTextColor;
                return;
            }

            bool isSelectedOrHovered = isSelected || isHovered;

            background.color = isPressed ? PressedBackgroundColor :
                isSelectedOrHovered ? HoveredBackgroundColor : NormalBackgroundColor;
            border.color     = NormalBorderColor;
            label.color      = NormalTextColor;
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

using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace StackBuild.UI.Scene.Title
{
    [RequireComponent(typeof(Button))]
    public class TitleMenuButton : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler
    {

        [SerializeField] private Button button;

        private void Reset()
        {
            button = GetComponent<Button>();
            button.transition = Selectable.Transition.None;
        }

        void ISelectHandler.OnSelect(BaseEventData eventData)
        {
            throw new NotImplementedException();
        }

        void IDeselectHandler.OnDeselect(BaseEventData eventData)
        {
            throw new NotImplementedException();
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            throw new NotImplementedException();
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            throw new NotImplementedException();
        }

    }
}
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;
using UniRx;

namespace StackBuild.UI
{
    public class DropdownAutoScroll : MonoBehaviour, IPointerClickHandler, ISubmitHandler
    {

        [SerializeField] private InputSystemUIInputModule inputModule;
        [SerializeField] private TMP_Dropdown dropdown;

        private float itemHeight;

        private void Reset()
        {
            dropdown = GetComponent<TMP_Dropdown>();
        }

        private void Start()
        {
            // キーボード/コントローラーで選んでいる (確定前) アイテムに自動スクロール
            // アイテムにOnSelectつけるとTMP_Dropdownの仕様でマウスでも反応するので
            // 上下の移動操作が来た時だけやる
            InputFromEvent.ActionMapFromEventPerformed(inputModule.move.action).Subscribe(ctx =>
            {
                if (!dropdown.IsExpanded) return;
                OnMoveInput(ctx.ReadValue<Vector2>().y);
            }).AddTo(this);

            itemHeight = dropdown.template.GetComponentInChildren<Toggle>(true).GetComponent<RectTransform>().sizeDelta
                .y;
        }

        private void OnOpen()
        {
            var scrollRect = dropdown.GetComponentInChildren<ScrollRect>();
            scrollRect.verticalNormalizedPosition =
                (float)(dropdown.options.Count - 1 - dropdown.value) / (dropdown.options.Count - 1);
        }

        private void OnMoveInput(float movement)
        {
            if (movement == 0) return;

            // currentSelectedGameObject更新前だけどこれもアイテムのはずだからあんまり気にしない
            var lastSelectedItem = EventSystem.current.currentSelectedGameObject.transform;
            // 念のためアイテムっぽいか確認 (TMP_Dropdown.DropdownItemは残念ながらprotected internal)
            if (lastSelectedItem.GetComponent<Toggle>() == null) return;

            // リストの最初にinactiveなアイテムがいるので-1
            int itemIdx = Mathf.Clamp(lastSelectedItem.GetSiblingIndex() - 1 - Math.Sign(movement), 0,
                dropdown.options.Count - 1);

            // 次に選択されるアイテムが表示範囲外ならギリ入るようにスクロール
            // (開くたびに新しいリストがInstantiateされていてめんどくなってきたのでここでGetComponentしちゃう)
            var scrollRect = lastSelectedItem.GetComponentInParent<ScrollRect>();
            float scrollRectHeight = scrollRect.GetComponent<RectTransform>().sizeDelta.y;
            float scrollableHeight = scrollRect.content.sizeDelta.y - scrollRectHeight;
            float scrollTop = scrollRect.verticalNormalizedPosition * scrollableHeight;
            float scrollBottom = scrollTop - scrollRectHeight;
            float itemTop = scrollableHeight - itemIdx * itemHeight;
            float itemBottom = itemTop - itemHeight;
            if (itemBottom < scrollBottom)
            {
                scrollRect.verticalNormalizedPosition = (itemBottom + scrollRectHeight) / scrollableHeight;
            }
            else if (itemTop > scrollTop)
            {
                scrollRect.verticalNormalizedPosition = itemTop / scrollableHeight;
            }
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData) => OnOpen();
        void ISubmitHandler.OnSubmit(BaseEventData eventData) => OnOpen();

    }
}

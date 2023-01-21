using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace StackBuild.UI
{
    [RequireComponent(typeof(Button))]
    public class CharacterSelectButton : MonoBehaviour
    {

        [SerializeField] private CharacterProperty character;
        [SerializeField] private Button button;
        [SerializeField] private RectTransform[] cornerIndicators;
        [SerializeField] private Image characterImage;
        [SerializeField] private TMP_Text nameText;

        public CharacterProperty Character
        {
            get => character;
            set
            {
                character = value;
                Display();
            }
        }
        public IObservable<Unit> OnClick => button.OnClickAsObservable();

        private void Reset()
        {
            button = GetComponent<Button>();
        }

        private void Awake()
        {
            Display();
        }

        private void Display()
        {
            if(character == null) return;
            characterImage.sprite = character.Sprite;
            nameText.text = character.Name;
        }

        public void SetCharacterSelected(bool selected)
        {
            foreach (var corner in cornerIndicators)
            {
                corner.localScale = selected ? Vector3.one : Vector3.zero;
            }
        }

    }
}

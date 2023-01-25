using System;
using StackBuild.Game;
using UnityEngine;
using UnityEngine.UI;

namespace StackBuild.Scene.Title
{
    [RequireComponent(typeof(Button))]
    public class GameModeButton : MonoBehaviour
    {

        [SerializeField] private Button button;
        [SerializeField] private GameMode gameMode;

        public GameMode GameMode => gameMode;
        public Button.ButtonClickedEvent OnClick => button.onClick;

        private void Reset()
        {
            button = GetComponent<Button>();
        }
    }
}

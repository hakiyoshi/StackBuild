using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using StackBuild.Game;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace StackBuild.Scene.Title
{

    public class MainMenuScreen : TitleSceneScreen
    {

        [SerializeField] private CanvasGroup container;
        [SerializeField] private TitleMenuStaggerDisplay staggerDisplay;
        [SerializeField] private GameModeButton[] gameModeButtons;
        [SerializeField] private Button buttonLocalMatch;
        [SerializeField] private Button buttonSettings;
        [SerializeField] private Button buttonBack;
        [SerializeField] private Button buttonExit;

        private readonly Subject<GameMode> onGameModeSelect = new();

        public IObservable<GameMode> OnGameModeSelect => onGameModeSelect;
        public Button.ButtonClickedEvent OnSettingsClick => buttonSettings.onClick;
        public Button.ButtonClickedEvent OnBackClick => buttonBack.onClick;
        public Button.ButtonClickedEvent OnExitClick => buttonExit.onClick;

        private void Awake()
        {
            foreach (var button in gameModeButtons)
            {
                button.OnClick.AddListener(() => onGameModeSelect.OnNext(button.GameMode));
            }
        }

        public override async UniTask ShowAsync()
        {
            container.interactable = true;
            container.blocksRaycasts = true;
            container.alpha = 1;

            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(buttonLocalMatch.gameObject);

            await staggerDisplay.Display();
        }

        public override async UniTask HideAsync()
        {
            container.interactable = false;
            container.blocksRaycasts = false;
            await container.DOFade(0, 0.07f);
            staggerDisplay.Hide();
        }

    }

}

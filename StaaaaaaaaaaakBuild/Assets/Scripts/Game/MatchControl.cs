using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using StackBuild.UI;
using UniRx;
using Unity.Netcode;
using UnityEngine;

namespace StackBuild.Game
{
    public class MatchControl : NetworkBehaviour
    {

        [Header("Appearance")]
        [SerializeField] private IntroDisplay introDisplay;
        [SerializeField] private float introDisplayDuration;
        [SerializeField] private CanvasGroup fade;
        [SerializeField] private float fadeIn;
        [SerializeField] private float fadeSustain;
        [SerializeField] private float fadeOut;
        [SerializeField] private HUDSlide[] huds;
        [SerializeField] private float hudDelay;
        [SerializeField] private StartDisplay startDisplay;
        [SerializeField] private float startDelay;
        [SerializeField] private TimeDisplay timeDisplay;
        [SerializeField] private int flashTimeBelow;
        [SerializeField] private FinishDisplay finishDisplay;
        [SerializeField] private ResultsDisplay resultsDisplay;
        [SerializeField] private float resultsDelay;
        [SerializeField] private GameOverScreen gameOverScreen;
        [Header("Game Parameters")]
        [SerializeField] private float gameTime;
        [Header("System")]
        [SerializeField] private PlayerInputProperty playerInputProperty;
        [SerializeField] private PlayerProperty[] players;
        [SerializeField] private MatchControlState matchControlState;

        private float timeRemaining;
        private readonly ReactiveProperty<MatchState> state = new();
        public IReadOnlyReactiveProperty<MatchState> State => state;

        private Dictionary<ulong, bool> playerWaitingToStart = new ();

        //サーバーにゲーム開始待ち通知をする
        [ServerRpc(RequireOwnership = true)]
        void SendWaitingToStartServerRpc(ulong playerIndex)
        {
            playerWaitingToStart.Add(playerIndex, true);
        }

        private void Start()
        {
            RunMatch(this.GetCancellationTokenOnDestroy()).Forget();
            state.AddTo(this);
        }

        public override void OnNetworkSpawn()
        {
            //ロビーに入ってる人のIDを受け取って登録
            var ids = NetworkManager.Singleton.ConnectedClientsIds;
            foreach (var id in ids)
            {
                playerWaitingToStart.Add(id, false);
            }
        }

        private async UniTaskVoid RunMatch(CancellationToken token)
        {
            state.Value = MatchState.Starting;
            matchControlState.SendState(MatchState.Starting);

            //最初のStackBuildが画面に映る
            DisablePlayerMovement();
            introDisplay.Display();
            timeDisplay.Display(Mathf.RoundToInt(gameTime));
            await UniTask.Delay(TimeSpan.FromSeconds(introDisplayDuration), cancellationToken: token);

            //オンライン時全員が同期するまで待ち
            if (IsSpawned)
            {
                await UniTask.WaitUntil(() =>
                {
                    foreach (var (_, flag) in playerWaitingToStart)
                    {
                        if (!flag)
                            return false;
                    }
                    return true;
                }, cancellationToken: token);
            }

            Debug.Log("ゲーム開始");
            //ホワイトアウト
            await fade.DOFade(1, fadeIn).From(0).SetEase(Ease.InQuad)
                .ToUniTask(cancellationToken: fade.gameObject.GetCancellationTokenOnDestroy());

            introDisplay.gameObject.SetActive(false);

            await UniTask.Delay(TimeSpan.FromSeconds(fadeSustain), cancellationToken: token);
            await fade.DOFade(0, fadeOut).ToUniTask(cancellationToken: fade.gameObject.GetCancellationTokenOnDestroy());

            //HUD表示
            await UniTask.Delay(TimeSpan.FromSeconds(hudDelay), cancellationToken: token);
            foreach (var hud in huds)
            {
                hud.SlideInAsync().Forget();
            }

            //ゲームスタート
            await UniTask.Delay(TimeSpan.FromSeconds(startDelay), cancellationToken: token);
            timeRemaining = gameTime;
            state.Value = MatchState.Ingame;
            matchControlState.SendState(MatchState.Ingame);
            EnablePlayerMovement();
            startDisplay.gameObject.SetActive(true);
            startDisplay.Display();
        }

        private async UniTaskVoid FinishMatch(CancellationToken token)
        {
            state.Value = MatchState.Finished;
            matchControlState.SendState(MatchState.Finished);
            DisablePlayerMovement();
            foreach (var hud in huds)
            {
                token.ThrowIfCancellationRequested();
                hud.SlideOutAsync().Forget();
            }
            finishDisplay.Display();

            await UniTask.Delay(TimeSpan.FromSeconds(resultsDelay), cancellationToken: token);
            await resultsDisplay.DisplayAsync(resultsDisplay.gameObject.GetCancellationTokenOnDestroy());
            gameOverScreen.ShowAsync(players.Select(p => p.characterProperty).ToArray()).Forget();
        }

        private void Update()
        {
            if (state.Value != MatchState.Ingame) return;

            int lastSeconds = Mathf.CeilToInt(timeRemaining);
            timeRemaining = Mathf.Max(0, timeRemaining - Time.deltaTime);
            int seconds = Mathf.CeilToInt(timeRemaining);
            if (seconds != lastSeconds)
            {
                timeDisplay.Display(seconds, seconds <= flashTimeBelow);
            }

            if (timeRemaining == 0)
            {
                FinishMatch(this.GetCancellationTokenOnDestroy()).Forget();
            }
        }

        private void DisablePlayerMovement()
        {
            // foreach (var input in playerInputProperty.PlayerInputs)
            // {
            //     if (input == null || input.gameObject == null) continue;
            //     input.gameObject.SetActive(false);
            // }
        }

        private void EnablePlayerMovement()
        {
            // for (int i = 0; i < PlayerInputProperty.MAX_DEVICEID; i++)
            // {
            //     var input = playerInputProperty.PlayerInputs[i];
            //     if (input == null || input.gameObject == null) continue;
            //     input.gameObject.SetActive(playerInputProperty.DeviceIds[i] != PlayerInputProperty.UNSETID);
            // }
        }

    }
}

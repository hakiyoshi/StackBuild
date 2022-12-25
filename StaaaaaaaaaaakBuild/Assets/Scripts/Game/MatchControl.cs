using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using NetworkSystem;
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

        [Header("Network")]
        [SerializeField] private int TimeoutSeconds = 600;
        [SerializeField] private LobbyManager lobbyManager;
        [SerializeField] private RelayManager relayManager;

        private float timeRemaining;
        private readonly ReactiveProperty<MatchState> state = new();
        public IReadOnlyReactiveProperty<MatchState> State => state;

        enum MatchStateSignal : int
        {
            Default = 0,
            GameStart = -1,
            GameFinish = -2,
        }
        // 0以上は待ち処理の人数
        // -1以下は合図のフラグ
        private int playerWaitingToSignal = 0;

        private void Start()
        {
            RunMatch(this.GetCancellationTokenOnDestroy()).Forget();
        }

        public override void OnNetworkSpawn()
        {
            playerWaitingToSignal = (int) MatchStateSignal.Default;
        }

        private async UniTaskVoid RunMatch(CancellationToken token)
        {
            state.Value = MatchState.Starting;
            matchControlState.SendState(MatchState.Starting);

            //最初のStackBuildが画面に映る
            introDisplay.Display();
            timeDisplay.Display(Mathf.RoundToInt(gameTime));
            await UniTask.Delay(TimeSpan.FromSeconds(introDisplayDuration), cancellationToken: token);

            //オンライン時全員が同期するまで待ち
            if (IsSpawned)
            {
                //開始待ちを送信して待つ
                SendStandbyStateServerRpc((int) MatchStateSignal.GameStart);
                await WaitForAllToSync((int) MatchStateSignal.GameStart, token);
            }

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
            startDisplay.gameObject.SetActive(true);
            startDisplay.Display();
        }

        private async UniTaskVoid FinishMatch(CancellationToken token)
        {
            state.Value = MatchState.Finished;
            matchControlState.SendState(MatchState.Finished);

            foreach (var hud in huds)
            {
                token.ThrowIfCancellationRequested();
                hud.SlideOutAsync().Forget();
            }
            finishDisplay.Display();

            //オンライン時全員が同期するまで待ち
            if (IsSpawned)
            {
                //開始待ちを送信して待つ
                SendStandbyStateServerRpc((int) MatchStateSignal.GameFinish);
                await WaitForAllToSync((int) MatchStateSignal.GameFinish, token);
            }

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

        // 指定の状態になるまで待ち
        async UniTask WaitForAllToSync(int signalState, CancellationToken token)
        {
            try
            {
                await UniTask.WaitUntil(() => playerWaitingToSignal == signalState,
                        cancellationToken: token)
                    .Timeout(TimeSpan.FromSeconds(TimeoutSeconds));
            }
            catch (TimeoutException)
            {
                //Stateをタイムアウトにする
                state.Value = MatchState.Timeout;
                matchControlState.SendState(MatchState.Timeout);
                NetworkSystemManager.NetworkExit(lobbyManager, relayManager).Forget();
                throw;
            }
            finally
            {
                playerWaitingToSignal = (int)MatchStateSignal.Default;
            }
        }



        //サーバーにゲーム開始待ち通知をする
        [ServerRpc(RequireOwnership = false)]
        void SendStandbyStateServerRpc(int signalState)
        {
            if (!IsServer)
                return;

            playerWaitingToSignal++;

            if (playerWaitingToSignal >= NetworkManager.Singleton.ConnectedClientsIds.Count)
            {
                SendMatchStateSignalServerRpc(NetworkManager.LocalTime.Time, signalState);
            }
        }

        // 待ち処理
        async UniTaskVoid WaitAndChangeGameState(int signalState, float timeToWait, CancellationToken token)
        {
            if (timeToWait > 0)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(timeToWait), cancellationToken: token);
            }

            playerWaitingToSignal = signalState;
        }

        //ゲーム状態変更通知
        [ServerRpc(RequireOwnership = false)]
        private void SendMatchStateSignalServerRpc(double time, int signalState)
        {
            if (!IsServer)
                return;

            SendMatchStateSignalClientRpc(time, signalState);
            var timeToWait = time - NetworkManager.ServerTime.Time;
            WaitAndChangeGameState(signalState, (float) timeToWait, this.GetCancellationTokenOnDestroy())
                .Forget();
        }

        [ClientRpc]
        private void SendMatchStateSignalClientRpc(double time, int signalState)
        {
            if (IsOwner)
                return;

            var timeToWait = time - NetworkManager.ServerTime.Time;
            WaitAndChangeGameState(signalState, (float) timeToWait, this.GetCancellationTokenOnDestroy())
                .Forget();
        }
    }
}

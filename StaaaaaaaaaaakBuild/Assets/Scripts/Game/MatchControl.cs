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
        [SerializeField] private int TimeoutSeconds = 600;
        [SerializeField] private PlayerInputProperty playerInputProperty;
        [SerializeField] private PlayerProperty[] players;
        [SerializeField] private MatchControlState matchControlState;

        [Header("Network")]
        [SerializeField] private LobbyManager lobbyManager;
        [SerializeField] private RelayManager relayManager;

        private float timeRemaining;
        private readonly ReactiveProperty<MatchState> state = new();
        public IReadOnlyReactiveProperty<MatchState> State => state;

        private const int GameStartSignalIndex = -1;
        private int numPlayerWaitingToStart = 0;//ゲーム開始待ちのプレイヤー人数(-1だと開始の合図)

        private void Start()
        {
            RunMatch(this.GetCancellationTokenOnDestroy()).Forget();
            state.AddTo(this);
        }

        public override void OnNetworkSpawn()
        {
            numPlayerWaitingToStart = 0;
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
                SendWaitingToStartServerRpc(NetworkManager.Singleton.LocalClientId);
                try
                {
                    await UniTask.WaitUntil(() => numPlayerWaitingToStart == GameStartSignalIndex, cancellationToken: token)
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

        //サーバーにゲーム開始待ち通知をする
        [ServerRpc(RequireOwnership = false)]
        void SendWaitingToStartServerRpc(ulong playerIndex)
        {
            if (!IsServer)
                return;

            numPlayerWaitingToStart++;

            //プレイヤー全員が待機になったら開始の合図
            if (numPlayerWaitingToStart >= NetworkManager.Singleton.ConnectedClientsIds.Count)
            {
                GameStartSignalServerRpc(NetworkManager.LocalTime.Time);
            }
        }

        //ゲームスタートの狼煙
        [ServerRpc]
        private void GameStartSignalServerRpc(double time)
        {
            if (!IsServer)
                return;

            GameStartSignalClientRpc(time);
            var timeToWait = time - NetworkManager.ServerTime.Time;
            WaitAndGameStart((float)timeToWait, this.GetCancellationTokenOnDestroy()).Forget();
        }

        [ClientRpc]
        private void GameStartSignalClientRpc(double time)
        {
            if (IsOwner)
                return;

            var timeToWait = time - NetworkManager.ServerTime.Time;
            WaitAndGameStart((float)timeToWait, this.GetCancellationTokenOnDestroy()).Forget();
        }

        private async UniTaskVoid WaitAndGameStart(float timeToWait, CancellationToken token)
        {
            if (timeToWait > 0)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(timeToWait), cancellationToken: token);
            }

            numPlayerWaitingToStart = GameStartSignalIndex;
        }
    }
}

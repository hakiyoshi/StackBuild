using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using NetworkSystem;
using StackBuild.Audio;
using StackBuild.UI;
using UniRx;
using Unity.Netcode;
using UnityEngine;

namespace StackBuild.Game
{
    public class MatchControl : SyncWaitingSystem
    {

        [Serializable]
        public struct PlayerInfo
        {
            [SerializeField] internal PlayerProperty player;
            [SerializeField] internal BuildingCore buildingCore;
            [SerializeField] internal HeightMeter heightMeter;
        }

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
        [SerializeField] private PlayerInfo[] players;
        [SerializeField] private MatchControlState matchControlState;

        [Header("Network")]
        [SerializeField] private int TimeoutSeconds = 600;
        [SerializeField] private LobbyManager lobbyManager;
        [SerializeField] private RelayManager relayManager;

        [Header("Audio")]
        [SerializeField] private AudioSourcePool audioSourcePool;

        [SerializeField] private AudioCue introStartCue;
        [SerializeField] private AudioCue introStackBuildCue;
        [SerializeField] private AudioCue gameStartCue;
        [SerializeField] private AudioCue gameCue;
        [SerializeField] private AudioCue gameEndCue;

        [SerializeField] private float delay = 3.0f;

        private AudioSourceWatching gameAudio = null;

        private float timeRemaining;
        private readonly ReactiveProperty<MatchState> state = new();
        public IReadOnlyReactiveProperty<MatchState> State => state;

        enum MatchStateSignal : int
        {
            Default = 0,
            GameStart = 1,
            GameFinish = 2,
        }

        private MatchStateSignal matchStateSignalServer = MatchStateSignal.Default;
        private MatchStateSignal matchStateSignal = MatchStateSignal.Default;

        private void Start()
        {
            foreach (var player in players)
            {
                player.buildingCore.TotalHeight.Subscribe(height => player.heightMeter.Set(height));
            }
            RunMatch(this.GetCancellationTokenOnDestroy()).Forget();
        }

        private async UniTaskVoid RunMatch(CancellationToken token)
        {
            //ネットワーク接続時サーバー限定でSignalを変えておく
            if (IsSpawned && IsServer)
                matchStateSignalServer = MatchStateSignal.GameStart;

            state.Value = MatchState.Starting;
            matchControlState.SendState(MatchState.Starting);

            await LoadingScreen.Instance.HideAsync(LoadingScreenType.Triangles);

            //イントロ開始時に流すBGM
            var introStart = audioSourcePool.Rent(introStartCue);
            introStart.audioSource.volume = 0.0f;
            introStart.audioSource.Play();
            introStart.audioSource.DOFade(0.5f, 1.0f);

            //StackBuildが表示される時のSE
            {
                DOVirtual.DelayedCall(0.4f, () => audioSourcePool.Rent(introStackBuildCue).PlayAndReturnWhenStopped());
            }

            //最初のStackBuildが画面に映る
            introDisplay.Display();
            timeDisplay.Display(Mathf.RoundToInt(gameTime));
            await UniTask.Delay(TimeSpan.FromSeconds(introDisplayDuration), cancellationToken: token);

            //オンライン時全員が同期するまで待ち
            if (IsSpawned)
            {
                //開始待ちを送信して待つ
                SendStandbyServerRpc();
                await WaitForAllToSync(MatchStateSignal.GameStart, token);
            }

            //イントロ開始時BGMを止める
            introStart.audioSource.DOFade(0.0f, 1.0f).OnKill(() =>
            {
                introStart.audioSource.Stop();
                introStart.ReturnAudio();
            });

            //ホワイトアウト
            await fade.DOFade(1, fadeIn).From(0).SetEase(Ease.InQuad)
                .ToUniTask(cancellationToken: fade.gameObject.GetCancellationTokenOnDestroy());

            introDisplay.gameObject.SetActive(false);
            introDisplay.DisableVirtualCamera();

            await UniTask.Delay(TimeSpan.FromSeconds(fadeSustain), cancellationToken: token);
            await fade.DOFade(0, fadeOut).ToUniTask(cancellationToken: fade.gameObject.GetCancellationTokenOnDestroy());

            //始まりサウンド
            audioSourcePool.Rent(gameStartCue).PlayAndReturnWhenStopped();

            //ゲームBGM再生
            {
                gameAudio = audioSourcePool.Rent(gameCue);
                gameAudio.audioSource.volume = 0.0f;
                gameAudio.audioSource.DOFade(0.15f, 2.5f);
                gameAudio.audioSource.Play();
            }

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
            //ネットワーク接続中かつサーバーの場合
            if (IsSpawned && IsServer)
                matchStateSignalServer = MatchStateSignal.GameFinish;

            //状態変更
            state.Value = MatchState.Finished;
            matchControlState.SendState(MatchState.Finished);

            //ゲーム終了サウンド
            audioSourcePool.Rent(gameEndCue).PlayAndReturnWhenStopped();

            //HUD表示
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
                SendStandbyServerRpc();
                await WaitForAllToSync(MatchStateSignal.GameFinish, token);
            }

            //ゲームBGM止める
            gameAudio.audioSource.DOFade(0.0f, 5.0f).OnKill(() =>
            {
                gameAudio.audioSource.Stop();
                gameAudio.ReturnAudio();
                gameAudio = null;
            });

            //リザルト表示
            await UniTask.Delay(TimeSpan.FromSeconds(resultsDelay), cancellationToken: token);
            await resultsDisplay.DisplayAsync(resultsDisplay.gameObject.GetCancellationTokenOnDestroy());
            gameOverScreen.ShowAsync(players.Select(p => p.player.characterProperty).ToArray()).Forget();
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
        async UniTask WaitForAllToSync(MatchStateSignal signalState, CancellationToken token)
        {
            try
            {
                await UniTask.WaitUntil(() => matchStateSignal == signalState,
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
                matchStateSignal = (int)MatchStateSignal.Default;
            }
        }

        protected override void OnSendStandby(int numWaitingToSignal)
        {
            if (numWaitingToSignal >= NetworkManager.ConnectedClientsIds.Count)
            {
                SendMatchStateSignalServerRpc(NetworkManager.LocalTime.Time, matchStateSignalServer);
            }
        }

        // 待ち処理
        async UniTaskVoid WaitAndChangeGameState(MatchStateSignal signalState, float timeToWait, CancellationToken token)
        {
            if (timeToWait > 0)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(timeToWait), cancellationToken: token);
            }

            matchStateSignal = signalState;
        }

        //ゲーム状態変更通知
        [ServerRpc(RequireOwnership = false)]
        private void SendMatchStateSignalServerRpc(double time, MatchStateSignal signalState)
        {
            if (!IsServer)
                return;

            SendMatchStateSignalClientRpc(time, signalState);
            var timeToWait = time - NetworkManager.ServerTime.Time;
            WaitAndChangeGameState(signalState, (float) timeToWait, this.GetCancellationTokenOnDestroy())
                .Forget();
        }

        [ClientRpc]
        private void SendMatchStateSignalClientRpc(double time, MatchStateSignal signalState)
        {
            if (IsOwner)
                return;

            var timeToWait = time - NetworkManager.ServerTime.Time;
            WaitAndChangeGameState(signalState, (float) timeToWait, this.GetCancellationTokenOnDestroy())
                .Forget();
        }
    }
}

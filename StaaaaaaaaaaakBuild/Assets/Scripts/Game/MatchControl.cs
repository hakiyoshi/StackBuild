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
    public class MatchControl : SyncWaitingSystem
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
            GameStart = 1,
            GameFinish = 2,
        }

        private MatchStateSignal matchStateSignalServer = MatchStateSignal.Default;
        private MatchStateSignal matchStateSignal = MatchStateSignal.Default;

        private void Start()
        {
            RunMatch(this.GetCancellationTokenOnDestroy()).Forget();
        }

        private async UniTaskVoid RunMatch(CancellationToken token)
        {
            //????????????????????????????????????????????????Signal??????????????????
            if (IsSpawned && IsServer)
                matchStateSignalServer = MatchStateSignal.GameStart;

            state.Value = MatchState.Starting;
            matchControlState.SendState(MatchState.Starting);

            //?????????StackBuild??????????????????
            introDisplay.Display();
            timeDisplay.Display(Mathf.RoundToInt(gameTime));
            await UniTask.Delay(TimeSpan.FromSeconds(introDisplayDuration), cancellationToken: token);

            //???????????????????????????????????????????????????
            if (IsSpawned)
            {
                //?????????????????????????????????
                SendStandbyServerRpc();
                await WaitForAllToSync(MatchStateSignal.GameStart, token);
            }

            //?????????????????????
            await fade.DOFade(1, fadeIn).From(0).SetEase(Ease.InQuad)
                .ToUniTask(cancellationToken: fade.gameObject.GetCancellationTokenOnDestroy());

            introDisplay.gameObject.SetActive(false);

            await UniTask.Delay(TimeSpan.FromSeconds(fadeSustain), cancellationToken: token);
            await fade.DOFade(0, fadeOut).ToUniTask(cancellationToken: fade.gameObject.GetCancellationTokenOnDestroy());

            //HUD??????
            await UniTask.Delay(TimeSpan.FromSeconds(hudDelay), cancellationToken: token);
            foreach (var hud in huds)
            {
                hud.SlideInAsync().Forget();
            }

            //?????????????????????
            await UniTask.Delay(TimeSpan.FromSeconds(startDelay), cancellationToken: token);
            timeRemaining = gameTime;
            state.Value = MatchState.Ingame;
            matchControlState.SendState(MatchState.Ingame);
            startDisplay.gameObject.SetActive(true);
            startDisplay.Display();
        }

        private async UniTaskVoid FinishMatch(CancellationToken token)
        {
            //??????????????????????????????????????????????????????
            if (IsSpawned && IsServer)
                matchStateSignalServer = MatchStateSignal.GameFinish;

            //????????????
            state.Value = MatchState.Finished;
            matchControlState.SendState(MatchState.Finished);

            //HUD??????
            foreach (var hud in huds)
            {
                token.ThrowIfCancellationRequested();
                hud.SlideOutAsync().Forget();
            }
            finishDisplay.Display();

            //???????????????????????????????????????????????????
            if (IsSpawned)
            {
                //?????????????????????????????????
                SendStandbyServerRpc();
                await WaitForAllToSync(MatchStateSignal.GameFinish, token);
            }

            //??????????????????
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

        // ????????????????????????????????????
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
                //State??????????????????????????????
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

        // ????????????
        async UniTaskVoid WaitAndChangeGameState(MatchStateSignal signalState, float timeToWait, CancellationToken token)
        {
            if (timeToWait > 0)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(timeToWait), cancellationToken: token);
            }

            matchStateSignal = signalState;
        }

        //???????????????????????????
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

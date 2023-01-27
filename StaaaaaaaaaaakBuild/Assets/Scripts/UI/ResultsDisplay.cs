using System;
using System.Linq;
using System.Threading;
using Cinemachine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using StackBuild.Audio;
using StackBuild.Extensions;
using UnityEngine;

namespace StackBuild.UI
{
    public class ResultsDisplay : MonoBehaviour
    {

        [Serializable]
        public struct PlayerInfo
        {
            [SerializeField] internal PlayerProperty player;
            [SerializeField] internal BuildingCore buildingCore;
            [SerializeField] internal Transform lookTarget;
            [SerializeField] internal ResultsHeightDisplay heightDisplay;
            internal float Height => buildingCore.TotalHeight.Value;
        }

        [SerializeField] private CinemachineVirtualCamera vcam;
        [SerializeField] private PlayerInfo[] players;
        [SerializeField] private float heightDisplayDelay;
        [SerializeField] private float heightDisplayDuration;
        [SerializeField] private float winDisplayDelay;
        [SerializeField] private float moveDelay;

        [Header("Audio")]
        [SerializeField] private AudioSourcePool audioSourcePool;
        [SerializeField] private AudioCue buildingCue;
        [SerializeField] private AudioCue winCue;

        public async UniTask DisplayAsync(CancellationToken token)
        {
            vcam.enabled = true;

            await UniTask.Delay(TimeSpan.FromSeconds(heightDisplayDelay), cancellationToken: token);

            var winner = players.Aggregate((a, b) => a.Height > b.Height ? a : b);
            foreach (var player in players)
            {
                player.heightDisplay.SetVisible(true, true);
            }

            //高さ表示のサウンド
            var buildingAudio = audioSourcePool.Rent(buildingCue);
            buildingAudio.audioSource.volume = 0.5f;
            buildingAudio.audioSource.DOFade(0.1f, 7.0f);
            buildingAudio.audioSource.Play();

            await DOVirtual.Float(0, winner.Height, heightDisplayDuration, y =>
            {
                foreach (var player in players)
                {
                    var playerY = Mathf.Min(y, player.Height);
                    var lookTarget = player.lookTarget.transform;
                    lookTarget.localPosition = lookTarget.localPosition.WithY(playerY);
                    player.heightDisplay.DisplayHeight(playerY);
                }
            }).SetEase(Ease.InQuad).ToUniTask(cancellationToken: token);

            //高さ表示のサウンド止める
            buildingAudio.audioSource.Stop();
            buildingAudio.ReturnAudio();

            await UniTask.Delay(TimeSpan.FromSeconds(winDisplayDelay), cancellationToken: token);

            DOVirtual.DelayedCall(0.4f, () => audioSourcePool.Rent(winCue).PlayAndReturnWhenStopped()).SetLink(gameObject);

            foreach (var player in players)
            {
                (player.Height == winner.Height
                    ? player.heightDisplay.DisplayWinAsync()
                    : player.heightDisplay.DisplayLoseAsync()).Forget();
            }

            await UniTask.Delay(TimeSpan.FromSeconds(moveDelay), cancellationToken: token);
            await players.Aggregate(DOTween.Sequence(),
                    (seq, player) =>
                        seq.Join(player.heightDisplay.transform.DOLocalMoveY(300, 0.65f).SetEase(Ease.InOutQuart)))
                .ToUniTask(cancellationToken: token);
        }

    }
}

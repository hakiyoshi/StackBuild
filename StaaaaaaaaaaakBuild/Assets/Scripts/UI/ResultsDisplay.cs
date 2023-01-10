using System;
using System.Linq;
using System.Threading;
using Cinemachine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
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

        public async UniTask DisplayAsync(CancellationToken token)
        {
            vcam.enabled = true;

            await UniTask.Delay(TimeSpan.FromSeconds(heightDisplayDelay), cancellationToken: token);

            var winner = players.Aggregate((a, b) => a.Height > b.Height ? a : b);
            foreach (var player in players)
            {
                player.heightDisplay.SetVisible(true, true);
            }

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

            await UniTask.Delay(TimeSpan.FromSeconds(winDisplayDelay), cancellationToken: token);

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

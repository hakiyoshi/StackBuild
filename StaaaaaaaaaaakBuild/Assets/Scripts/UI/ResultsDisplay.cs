using System;
using System.Linq;
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
            internal float Height => buildingCore.TotalHeight;
        }

        [SerializeField] private CinemachineVirtualCamera vcam;
        [SerializeField] private PlayerInfo[] players;
        [SerializeField] private float heightDisplayDelay;
        [SerializeField] private float heightDisplayDuration;
        [SerializeField] private float winDisplayDelay;

        public async UniTask DisplayAsync()
        {
            vcam.enabled = true;

            await UniTask.Delay(TimeSpan.FromSeconds(heightDisplayDelay));

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
            }).SetEase(Ease.InQuad);

            await UniTask.Delay(TimeSpan.FromSeconds(winDisplayDelay));

            foreach (var player in players)
            {
                (player.Height == winner.Height
                    ? player.heightDisplay.DisplayWinAsync()
                    : player.heightDisplay.DisplayLoseAsync()).Forget();
            }
        }

    }
}

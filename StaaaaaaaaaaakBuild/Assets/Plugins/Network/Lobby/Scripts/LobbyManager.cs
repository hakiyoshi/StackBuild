using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace NetworkSystem
{
    [CreateAssetMenu(menuName = "Network/Lobby/LobbyManager", fileName = "LobbyManager")]

    public class LobbyManager : ScriptableObject
    {
        public const string DefaultLobbyName = "PlayerLobby";
        public const int DefaultMaxPlayer = 4;
        public const bool DefaultIsPrivate = false;

        private const double CoolTimeInitialValue = 1;
        private const double CoolTimeMultiplyValue = 2.5;
        private const double CoolTimeMaxValue = 30;
        private const double HeartbeatTransmissionTime = 15.1; // 待ち時間(秒) ※30秒間に5回の制限あり

        private Lobby lobby;

        public Lobby lobbyInfo
        {
            get
            {
                return lobby;
            }

            set
            {
                if (value == null)
                    return;

                lobby = value;
            }
        }


        public enum SettingEvent
        {
            Create,
            LobbyUpdate,
            PlayerUpdate,
            Join,
            Exit,
        }
        public IObservable<SettingEvent> OnLobbySetting => onLobbySetting;
        private Subject<SettingEvent> onLobbySetting = new Subject<SettingEvent>();

        public enum LobbyStatus
        {
            NonPerticipation,
            Host,
            Client,
        }
        public LobbyStatus Status { get; private set; } = LobbyStatus.NonPerticipation;

        private CancellationTokenSource heartbeatCts = null;
        private CancellationTokenSource latestLobbyAcquisitionCts = null;
        private void CreateCancellationTokenSource(ref CancellationTokenSource cts)
        {
            if (cts != null)
            {
                cts.Cancel();
                cts.Dispose();
            }

            cts = new CancellationTokenSource();
        }

        private void CancelCancellationTokenSource(ref CancellationTokenSource cts)
        {
            if(cts != null)
            {
                cts.Cancel();
                cts.Dispose();
            }

            cts = null;
        }

        //---------------------------------------------------------------------------
        // ロビー作成

        public async UniTask CreateLobbyAsync(string lobbyName, int maxPlayer, bool isPrivate, LobbyOption lobbyOption, PlayerOption playerOption)
        {
            if (lobby != null)
                return;

            try
            {
                var options = new CreateLobbyOptions
                {
                    IsPrivate = isPrivate,
                    Data = lobbyOption.GetPlayerOptions(),
                    Player = new Player()
                    {
                        Data = playerOption.GetPlayerOptions(),
                    }
                };

                lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayer, options);

                //ロビー更新系
                CreateCancellationTokenSource(ref latestLobbyAcquisitionCts);
                LatestLobbyAcquisitionLoopAsync(latestLobbyAcquisitionCts.Token).Forget();

                //ハートビート
                CreateCancellationTokenSource(ref heartbeatCts);
                HeartbeatAsync(heartbeatCts.Token).Forget();

                Status = LobbyStatus.Host;
                onLobbySetting.OnNext(SettingEvent.Create);
                Debug.Log("ロビー作成");
            }
            catch (LobbyServiceException e)
            {
                Debug.LogException(e);
                throw;
            }
        }

        //---------------------------------------------------------------------------
        // ロビー情報更新

        public async UniTask UpdateLobbyDataAsync(
            bool isLocked,
            string lobbyName,
            int maxPlayer,
            bool isPrivate,
            LobbyOption lobbyOption)
        {
            var options = new UpdateLobbyOptions
            {
                Name = lobbyName,
                MaxPlayers = maxPlayer,
                IsPrivate = isPrivate,
                IsLocked = isLocked,
                Data = lobbyOption.GetPlayerOptions()
            };

            await UpdateLobbyDataAsync(options);
        }


        //基本設定のみそのままでOptionのみ変更
        public async UniTask UpdateLobbyDataAsync(LobbyOption lobbyOption)
        {
            var options = new UpdateLobbyOptions
            {
                Name = lobby.Name,
                MaxPlayers = lobby.MaxPlayers,
                IsPrivate = lobby.IsPrivate,
                IsLocked = lobby.IsLocked,
                Data = lobbyOption.GetPlayerOptions()
            };

            await UpdateLobbyDataAsync(options);
        }

        public async UniTask UpdateLobbyDataAsync(UpdateLobbyOptions options)
        {
            if (lobby == null)
                return;

            try
            {
                lobby = await LobbyService.Instance.UpdateLobbyAsync(lobby.Id, options);

                onLobbySetting.OnNext(SettingEvent.LobbyUpdate);
                Debug.Log("ロビー情報の更新をしました");
            }
            catch (LobbyServiceException e)
            {
                Debug.LogException(e);
                throw;
            }
        }

        //---------------------------------------------------------------------------
        // クイック入室

        public async UniTask QuickJoinAsync(int maxPlayer)
        {
            if(lobby != null)
                return;

            try
            {
                var options = new QuickJoinLobbyOptions()
                {
                    Filter = new List<QueryFilter>()
                    {
                        new QueryFilter(//プレイヤー数がvalue未満のロビーを探す
                            field: QueryFilter.FieldOptions.MaxPlayers,
                            op: QueryFilter.OpOptions.GE,
                            value: maxPlayer.ToString())
                    }
                };

                lobby = await LobbyService.Instance.QuickJoinLobbyAsync(options);

                //更新系
                CreateCancellationTokenSource(ref latestLobbyAcquisitionCts);
                LatestLobbyAcquisitionLoopAsync(latestLobbyAcquisitionCts.Token).Forget();

                Status = LobbyStatus.Client;
                onLobbySetting.OnNext(SettingEvent.Join);
                Debug.Log("クイック入室");
            }
            catch (LobbyServiceException e)
            {
                Debug.LogException(e);
                throw;
            }
        }

        //---------------------------------------------------------------------------
        //ロビーIDから入室

        public async UniTask JoinLobbybyCodeAsync(string lobbyCode)
        {
            if(lobby != null)
                return;

            try
            {
                lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);

                CreateCancellationTokenSource(ref latestLobbyAcquisitionCts);
                LatestLobbyAcquisitionLoopAsync(latestLobbyAcquisitionCts.Token).Forget();

                Status = LobbyStatus.Client;
                onLobbySetting.OnNext(SettingEvent.Join);
                Debug.Log("Codeで入室");
            }
            catch (LobbyServiceException e)
            {
                Debug.LogException(e);
                throw;
            }
        }

        //---------------------------------------------------------------------------
        // 条件指定してロビーリストを取得

        // public async UniTask QueryLobbiesAsync(LobbyOption option)
        // {
        //
        // }


        //---------------------------------------------------------------------------
        //プレイヤーデータを更新

        public async UniTask UpdatePlayerDataAsync(PlayerOption playerOption)
        {
            try
            {
                var options = new UpdatePlayerOptions
                {
                    Data = playerOption.GetPlayerOptions()
                };

                var playerId = AuthenticationService.Instance.PlayerId;

                lobby = await LobbyService.Instance.UpdatePlayerAsync(lobby.Id, playerId, options);

                onLobbySetting.OnNext(SettingEvent.PlayerUpdate);
                Debug.Log("プレイヤーデータを更新しました");
            }
            catch (LobbyServiceException e)
            {
                Debug.LogException(e);
                throw;
            }
        }

        //---------------------------------------------------------------------------
        //ロビー情報常時更新

        private async UniTask LatestLobbyAcquisitionLoopAsync(CancellationToken token)
        {
            Debug.Log("手持ちのロビー情報更新開始");

            double cooltime = CoolTimeInitialValue;
            while (lobby != null)
            {
                try
                {
                    await UniTask.Delay(TimeSpan.FromSeconds(cooltime), cancellationToken: token);
                    cooltime = Math.Min(cooltime * CoolTimeMultiplyValue, CoolTimeMaxValue);

                    lobby = await LobbyService.Instance.GetLobbyAsync(lobby.Id);
                }
                catch (ArgumentException e)
                {
                    Debug.LogException(e);
                    break;
                }
                catch (LobbyServiceException e)
                {
                    Debug.LogException(e);
                    break;
                }
                catch (OperationCanceledException e)
                {
                    //Debug.LogException(e);
                    break;
                }
            }
            Debug.Log("手持ちのロビー情報更新終了");
        }

        // ロビー情報更新処理を止める
        public void StopLatestLobbyAcquisitionLoop()
        {
            CancelCancellationTokenSource(ref latestLobbyAcquisitionCts);
        }

        //---------------------------------------------------------------------------
        //ハートビート

        private async UniTask HeartbeatAsync(CancellationToken token)
        {
            Debug.Log("ハートビート開始");
            while (lobby != null)
            {
                try
                {
                    await UniTask.Delay(TimeSpan.FromSeconds(HeartbeatTransmissionTime), cancellationToken: token);
                    await LobbyService.Instance.SendHeartbeatPingAsync(lobby.Id);
                }
                catch (LobbyServiceException e)
                {
                    Debug.LogException(e);
                    break;
                }
                catch (OperationCanceledException e)
                {
                    //Debug.LogException(e);
                    break;
                }
            }
            Debug.Log("ハートビート終了");
        }

        //ハートビートを止める
        public void StopHeartbeat()
        {
            CancelCancellationTokenSource(ref heartbeatCts);
        }

        //---------------------------------------------------------------------------
        //ロビー退出

        public async UniTask LobbyExit()
        {
            if (lobby == null)
                return;

            try
            {
                //更新系を終わらせる
                CancelCancellationTokenSource(ref heartbeatCts);
                CancelCancellationTokenSource(ref latestLobbyAcquisitionCts);

                await LobbyService.Instance.RemovePlayerAsync(lobby.Id, AuthenticationService.Instance.PlayerId);

                Status = LobbyStatus.NonPerticipation;
                onLobbySetting.OnNext(SettingEvent.Exit);
                Debug.Log("ロビーから退出");
            }
            catch (ArgumentNullException e)
            {
                Debug.LogException(e);
                throw;
            }
            catch (LobbyServiceException e)
            {
                Debug.LogException(e);
                throw;
            }
            finally
            {
                lobby = null;
            }
        }

        //---------------------------------------------------------------------------
    }

}
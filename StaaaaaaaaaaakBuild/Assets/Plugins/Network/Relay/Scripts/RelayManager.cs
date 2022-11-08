using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UniRx;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

namespace NetworkSystem
{
    [CreateAssetMenu(menuName = "Network/Relay/RelayManager", fileName = "RelayManager")]

    public class RelayManager : ScriptableObject
    {
        public enum SettingEvent{
            Create,
            Join,
            Exit,
        }
        public IObservable<SettingEvent> OnRelaySetting => onRelaySetting;
        private Subject<SettingEvent> onRelaySetting = new Subject<SettingEvent>();

        //ホストクライアント共通
        public string IPV4Address { get; private set; }
        public ushort port { get; private set; }
        public byte[] allocationIdBytes { get; private set; }
        public byte[] connectionData { get; private set; }
        public byte[] key { get; private set; }
        public string JoinCode { get; private set; }
        public string region { get; private set; }
        
        
        //クライアント限定
        public byte[] hostConnectionData { get; private set; }
        
        //--------------------------------------------------------------------------------

        // 利用可能なすべてのリレーサーバーの地域をリストアップする
        public static async UniTask<List<Region>> ListupRegionsAsync()
        {
            try
            {
                return await RelayService.Instance.ListRegionsAsync();
            }
            catch (Exception e)
            {
                throw new Exception("List regions request failed" + e.Message);
            }
        }

        private static async UniTask<Allocation> CreateAllocationAsync(int maxConnections, string targetRegionId)
        {
            try
            {
                return await RelayService.Instance.CreateAllocationAsync(maxConnections, targetRegionId);
            }
            catch (Exception e)
            {
                throw new Exception("Relay create allocation request failed " + e.Message);
            }
        }
        
        // 利用可能なリレーサーバーから最適なサーバーを自動的に選択する
        private static async UniTask<Allocation> CreateAllocationAsyncQoS(int maxConnections)
        {
            try
            {
                return await RelayService.Instance.CreateAllocationAsync(maxConnections);
            }
            catch (Exception e)
            {
                throw new Exception("Relay create allocation request failed " + e.Message);
            }
        }

        private static async UniTask<string> GetJoinCodeAsync(Guid allocationId)
        {
            try
            {
                return await RelayService.Instance.GetJoinCodeAsync(allocationId);
            }
            catch (Exception e)
            {
                throw new Exception("Relay create join code request failed" + e.Message);
            }
        }

        private async UniTask SetupVariable(Allocation allocation)
        {
            JoinCode = await GetJoinCodeAsync(allocation.AllocationId);
            
            var dtlsEndpoint = allocation.ServerEndpoints.First(e => e.ConnectionType == "dtls");
            IPV4Address = dtlsEndpoint.Host;
            port = (ushort) dtlsEndpoint.Port;
            allocationIdBytes = allocation.AllocationIdBytes;
            connectionData = allocation.ConnectionData;
            key = allocation.Key;
            region = allocation.Region;
        }

        //--------------------------------------------------------------------------------
        //Allocation生成

        public async UniTask CreateAllocationAsync(int maxConnections)
        {
            try
            {
                CheckMaxConnections(maxConnections);
                
                var allocation = await CreateAllocationAsyncQoS(maxConnections);

                await SetupVariable(allocation);

                //ログ
                Debug.Log($"ConnectionData: {allocation.ConnectionData[0]} {allocation.ConnectionData[1]}" + 
                          $"AllocationId {allocation.AllocationId}");

                //ネットワークマネージャーに入れる
                Unity.Netcode.NetworkManager.Singleton.GetComponent<UnityTransport>().
                    SetHostRelayData(IPV4Address, port, allocationIdBytes, key, connectionData, true);
                
                if(!Unity.Netcode.NetworkManager.Singleton.StartHost())
                    throw new Exception("StartHost failed.");
                
                onRelaySetting.OnNext(SettingEvent.Create);
                Debug.Log("Relayアロケーションを作成");
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
        }

        public async UniTask CreateAllocationAsync(int maxConnections, Region targetRegion)
        {
            try
            {
                CheckMaxConnections(maxConnections);
                
                var allocation = await CreateAllocationAsync(maxConnections, targetRegion.Id);

                await SetupVariable(allocation);

                //ログ
                Debug.Log($"ConnectionData: {allocation.ConnectionData[0]} {allocation.ConnectionData[1]}" + 
                          $"AllocationId {allocation.AllocationId}");

                //ネットワークマネージャーに入れる
                Unity.Netcode.NetworkManager.Singleton.GetComponent<UnityTransport>().
                    SetHostRelayData(IPV4Address, port, allocationIdBytes, key, connectionData, true);
                
                if(!Unity.Netcode.NetworkManager.Singleton.StartHost())
                    throw new Exception("StartHost failed.");
                
                onRelaySetting.OnNext(SettingEvent.Create);
                Debug.Log("Relayアロケーションを作成");
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
        }

        private static void CheckMaxConnections(int maxConnections)
        {
#if UNITY_EDITOR
            switch (maxConnections)
            {
                case <= 0:
                    throw new Exception("maxConnections <= 0");
                case >= 10:
                    throw new Exception("maxConnections >= 10");
            }
#endif
        }

        //--------------------------------------------------------------------------------
        //Allocation参加
        
        public async UniTask JoinAllocationAsync(string joinCode)
        {
            try
            {
                var allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
                Debug.Log($"client connection data: {allocation.ConnectionData[0]} {allocation.ConnectionData[1]}");
                Debug.Log($"host connection data: {allocation.HostConnectionData[0]} {allocation.HostConnectionData[1]}");
                Debug.Log($"client allocation ID: {allocation.AllocationId}");

                var dtlsEndpoint = allocation.ServerEndpoints.First(e => e.ConnectionType == "dtls");
                IPV4Address = dtlsEndpoint.Host;
                port = (ushort) dtlsEndpoint.Port;
                allocationIdBytes = allocation.AllocationIdBytes;
                connectionData = allocation.ConnectionData;
                key = allocation.Key;
                hostConnectionData = allocation.HostConnectionData;
                JoinCode = joinCode;
                
                //ネットワークマネージャーに入れる
                Unity.Netcode.NetworkManager.Singleton.GetComponent<UnityTransport>().
                    SetClientRelayData(IPV4Address, port, allocationIdBytes, key, connectionData, hostConnectionData, true);
                if (!Unity.Netcode.NetworkManager.Singleton.StartClient())
                    throw new Exception("StartClient failed.");
                
                onRelaySetting.OnNext(SettingEvent.Join);
                Debug.Log("Relayアロケーションに参加");
            }
            catch (Exception e)
            {
                Debug.LogError("Relay join request failed" + e.Message);
                throw;
            }
        }
        
        //--------------------------------------------------------------------------------
        // 退出

        public void RelayExit()
        {
            Unity.Netcode.NetworkManager.Singleton.Shutdown(true);
            onRelaySetting.OnNext(SettingEvent.Exit);
            Debug.Log("Relayアロケーションを退出しました");
        }
    }
}

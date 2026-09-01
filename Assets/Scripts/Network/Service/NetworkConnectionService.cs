using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

namespace FireLine.Scripts.Network.Service
{
    public class NetworkConnectionService
    {
        private readonly NetworkManager _networkManager;

        private const string ConnectionType = "dtls";

        public NetworkConnectionService(
            NetworkManager networkManager)
        {
            _networkManager = networkManager;
        }

        public async Task<string> StartHostWithRelay(
            int maxConnections)
        {
            if (_networkManager == null)
            {
                Debug.LogError(
                    "[NETWORK CONNECTION] " +
                    "NetworkManager is NULL!"
                );

                return null;
            }

            if (_networkManager.IsListening)
            {
                Debug.LogWarning(
                    "[NETWORK CONNECTION] " +
                    "NetworkManager is already listening."
                );

                return null;
            }

            try
            {
                Debug.Log(
                    "[NETWORK CONNECTION] " +
                    "Creating Relay allocation..."
                );

                Allocation allocation =
                    await RelayService.Instance
                        .CreateAllocationAsync(
                            maxConnections
                        );

                UnityTransport transport =
                    _networkManager
                        .GetComponent<UnityTransport>();

                if (transport == null)
                {
                    Debug.LogError(
                        "[NETWORK CONNECTION] " +
                        "UnityTransport not found!"
                    );

                    return null;
                }

                transport.SetRelayServerData(
                    AllocationUtils.ToRelayServerData(
                        allocation,
                        ConnectionType
                    )
                );

                string joinCode =
                    await RelayService.Instance
                        .GetJoinCodeAsync(
                            allocation.AllocationId
                        );

                bool started =
                    _networkManager.StartHost();

                if (!started)
                {
                    Debug.LogError(
                        "[NETWORK CONNECTION] " +
                        "StartHost failed!"
                    );

                    return null;
                }

                Debug.Log(
                    "[NETWORK CONNECTION] " +
                    "Relay Host started | " +
                    $"JoinCode: {joinCode}"
                );

                return joinCode;
            }
            catch (System.Exception exception)
            {
                Debug.LogError(
                    "[NETWORK CONNECTION] " +
                    $"Host Relay failed | {exception}"
                );

                return null;
            }
        }

        public async Task<bool> StartClientWithRelay(
            string joinCode)
        {
            if (_networkManager == null)
            {
                Debug.LogError(
                    "[NETWORK CONNECTION] " +
                    "NetworkManager is NULL!"
                );

                return false;
            }

            if (string.IsNullOrWhiteSpace(joinCode))
            {
                Debug.LogError(
                    "[NETWORK CONNECTION] " +
                    "Join code is empty!"
                );

                return false;
            }

            if (_networkManager.IsListening)
            {
                Debug.LogWarning(
                    "[NETWORK CONNECTION] " +
                    "NetworkManager is already listening."
                );

                return false;
            }

            try
            {
                Debug.Log(
                    "[NETWORK CONNECTION] " +
                    $"Joining Relay | Code: {joinCode}"
                );

                JoinAllocation allocation =
                    await RelayService.Instance
                        .JoinAllocationAsync(
                            joinCode.Trim()
                        );

                UnityTransport transport =
                    _networkManager
                        .GetComponent<UnityTransport>();

                if (transport == null)
                {
                    Debug.LogError(
                        "[NETWORK CONNECTION] " +
                        "UnityTransport not found!"
                    );

                    return false;
                }

                transport.SetRelayServerData(
                    AllocationUtils.ToRelayServerData(
                        allocation,
                        ConnectionType
                    )
                );

                bool started =
                    _networkManager.StartClient();

                Debug.Log(
                    "[NETWORK CONNECTION] " +
                    $"StartClient | Success: {started}"
                );

                return started;
            }
            catch (System.Exception exception)
            {
                Debug.LogError(
                    "[NETWORK CONNECTION] " +
                    $"Client Relay failed | {exception}"
                );

                return false;
            }
        }

        public void Shutdown()
        {
            if (_networkManager == null)
                return;

            if (!_networkManager.IsListening)
                return;

            Debug.Log(
                "[NETWORK CONNECTION] Shutdown."
            );

            _networkManager.Shutdown();
        }
    }
}
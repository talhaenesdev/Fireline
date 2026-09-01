using System;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Services.Multiplayer;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FireLine.Scripts.Network.Service
{
    public class NetworkGameStartService
    {
        private const string GameSceneName = "Game";

        public async Task<bool> StartGame(
            ISession session)
        {
            if (session == null)
            {
                Debug.LogError(
                    "[GAME START] Session is NULL!"
                );

                return false;
            }

            if (!session.IsHost)
            {
                Debug.LogWarning(
                    "[GAME START] Only host can start game."
                );

                return false;
            }

            try
            {
                Debug.Log(
                    "[GAME START] " +
                    "Starting Relay network..."
                );

                RelayNetworkOptions networkOptions =
                    RelayNetworkOptions.Default;

                await session
                    .AsHost()
                    .Network
                    .StartRelayNetworkAsync(
                        networkOptions
                    );

                Debug.Log(
                    "[GAME START] " +
                    "Relay network started."
                );

                NetworkManager networkManager =
                    NetworkManager.Singleton;

                if (networkManager == null)
                {
                    Debug.LogError(
                        "[GAME START] " +
                        "NetworkManager.Singleton is NULL!"
                    );

                    return false;
                }

                Debug.Log(
                    $"[GAME START] Network state | " +
                    $"IsServer: {networkManager.IsServer} | " +
                    $"IsHost: {networkManager.IsHost} | " +
                    $"IsClient: {networkManager.IsClient}"
                );

                if (!networkManager.IsServer)
                {
                    Debug.LogError(
                        "[GAME START] " +
                        "Relay started but NGO is not Server!"
                    );

                    return false;
                }

                if (networkManager.SceneManager == null)
                {
                    Debug.LogError(
                        "[GAME START] " +
                        "NetworkSceneManager is NULL!"
                    );

                    return false;
                }

                AsyncOperationStatus(
                    networkManager
                );

                return true;
            }
            catch (Exception exception)
            {
                Debug.LogError(
                    $"[GAME START] " +
                    $"Failed to start game | {exception}"
                );

                return false;
            }
        }

        private void AsyncOperationStatus(
            NetworkManager networkManager)
        {
            Debug.Log(
                $"[GAME START] " +
                $"Loading scene: {GameSceneName}"
            );

            SceneEventProgressStatus status =
                networkManager.SceneManager.LoadScene(
                    GameSceneName,
                    LoadSceneMode.Single
                );

            Debug.Log(
                $"[GAME START] " +
                $"Scene load request status: {status}"
            );
        }
    }
}
using System;
using System.Threading.Tasks;
using FireLine.Scripts.Core.Scene.Model;
using Unity.Netcode;
using Unity.Services.Multiplayer;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace FireLine.Scripts.Network.Service
{
    public class NetworkGameStartService
    {
        private readonly SceneReference _gameScene;

        public NetworkGameStartService(
            [Inject(Id = "GameScene")]
            SceneReference gameScene)
        {
            _gameScene = gameScene;
        }

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

            if (_gameScene == null)
            {
                Debug.LogError(
                    "[GAME START] " +
                    "Game SceneReference is NULL!"
                );

                return false;
            }

            if (!_gameScene.IsValid)
            {
                Debug.LogError(
                    "[GAME START] " +
                    "Game SceneReference is invalid!"
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

                LoadGameScene(
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

        private void LoadGameScene(
            NetworkManager networkManager)
        {
            Debug.Log(
                $"[GAME START] " +
                $"Loading scene: {_gameScene.SceneName}"
            );

            SceneEventProgressStatus status =
                networkManager.SceneManager.LoadScene(
                    _gameScene.SceneName,
                    LoadSceneMode.Single
                );

            Debug.Log(
                $"[GAME START] " +
                $"Scene load request status: {status}"
            );
        }
    }
}
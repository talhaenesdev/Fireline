using System;
using System.Collections;
using FireLine.Scripts.Network.Signals;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace FireLine.Scripts.Network.Service
{
    public class NetworkPlayerRespawnService : IInitializable, IDisposable
    {
        private readonly SignalBus _signalBus;
        private readonly NetworkManager _networkManager;
        private readonly NetworkSpawnPointManager _spawnPointManager;
        private readonly NetworkCoroutineRunner _coroutineRunner;

        private const float RespawnDelay = 2f;

        public NetworkPlayerRespawnService(
            SignalBus signalBus,
            NetworkManager networkManager,
            NetworkSpawnPointManager spawnPointManager,
            NetworkCoroutineRunner coroutineRunner)
        {
            _signalBus = signalBus;
            _networkManager = networkManager;
            _spawnPointManager = spawnPointManager;
            _coroutineRunner = coroutineRunner;
        }

        public void Initialize()
        {
            Debug.Log(
                $"[RESPAWN SERVICE] Initialize | " +
                $"IsServer: {_networkManager.IsServer} | " +
                $"IsClient: {_networkManager.IsClient} | " +
                $"IsHost: {_networkManager.IsHost}"
            );

            if (!_networkManager.IsServer)
                return;

            _signalBus.Subscribe<NetworkPlayerDeathSignal>(
                OnPlayerDeath
            );

            Debug.Log(
                "[RESPAWN SERVICE] " +
                "Subscribed to NetworkPlayerDeathSignal"
            );
        }

        public void Dispose()
        {
            if (_signalBus == null ||
                _networkManager == null)
                return;

            if (!_networkManager.IsServer)
                return;

            _signalBus.TryUnsubscribe<NetworkPlayerDeathSignal>(
                OnPlayerDeath
            );

            Debug.Log(
                "[RESPAWN SERVICE] " +
                "Unsubscribed from NetworkPlayerDeathSignal"
            );
        }

        private void OnPlayerDeath(
            NetworkPlayerDeathSignal signal)
        {
            if (!_networkManager.IsServer)
                return;

            Debug.Log(
                $"[RESPAWN SERVICE] " +
                $"Death signal received | " +
                $"ClientId: {signal.ClientId}"
            );

            _coroutineRunner.StartCoroutine(
                RespawnCoroutine(signal.ClientId)
            );
        }

        private IEnumerator RespawnCoroutine(
            ulong clientId)
        {
            Debug.Log(
                $"[RESPAWN SERVICE] " +
                $"Waiting {RespawnDelay} seconds | " +
                $"ClientId: {clientId}"
            );

            yield return new WaitForSeconds(
                RespawnDelay
            );

            RespawnPlayer(clientId);
        }

        private void RespawnPlayer(
            ulong clientId)
        {
            if (!_networkManager.IsServer)
                return;

            Debug.Log(
                $"[RESPAWN SERVICE] " +
                $"RespawnPlayer | ClientId: {clientId}"
            );

            if (!_networkManager.ConnectedClients.TryGetValue(
                    clientId,
                    out NetworkClient client))
            {
                Debug.LogError(
                    $"[RESPAWN SERVICE] " +
                    $"Client not found | ClientId: {clientId}"
                );

                return;
            }

            NetworkObject oldPlayer =
                client.PlayerObject;

            if (oldPlayer != null &&
                oldPlayer.IsSpawned)
            {
                Debug.Log(
                    $"[RESPAWN SERVICE] " +
                    $"Despawning old player | " +
                    $"ClientId: {clientId}"
                );

                oldPlayer.Despawn(true);
            }

            Transform spawnPoint =
                _spawnPointManager.GetSpawnPoint(
                    clientId
                );

            if (spawnPoint == null)
            {
                Debug.LogError(
                    "[RESPAWN SERVICE] " +
                    "No respawn point found!"
                );

                return;
            }

            GameObject playerPrefab =
                _networkManager.NetworkConfig.PlayerPrefab;

            if (playerPrefab == null)
            {
                Debug.LogError(
                    "[RESPAWN SERVICE] " +
                    "PlayerPrefab is NULL!"
                );

                return;
            }

            GameObject newPlayer =
                UnityEngine.Object.Instantiate(
                    playerPrefab,
                    spawnPoint.position,
                    spawnPoint.rotation
                );

            NetworkObject networkObject =
                newPlayer.GetComponent<NetworkObject>();

            if (networkObject == null)
            {
                Debug.LogError(
                    "[RESPAWN SERVICE] " +
                    "NetworkObject missing on PlayerPrefab!"
                );

                UnityEngine.Object.Destroy(
                    newPlayer
                );

                return;
            }

            networkObject.SpawnAsPlayerObject(
                clientId,
                true
            );

            Debug.Log(
                $"[RESPAWN SERVICE] " +
                $"Player respawned successfully | " +
                $"ClientId: {clientId}"
            );
        }
    }
}
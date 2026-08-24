using System;
using System.Collections;
using FireLine.Scripts.Network.Signals;
using FireLine.Scripts.Network.Service;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace FireLine.Scripts.Network.Service
{
    public class NetworkPlayerRespawnService :
        IInitializable,
        IDisposable
    {
        private readonly SignalBus _signalBus;
        private readonly NetworkManager _networkManager;
        private readonly NetworkPlayer _playerPrefab;
        private readonly NetworkSpawnPointManager _spawnPointManager;

        private bool _isSubscribed;

        private const float RespawnDelay = 2f;

        public NetworkPlayerRespawnService(
            SignalBus signalBus,
            NetworkManager networkManager,
            NetworkPlayer playerPrefab,
            NetworkSpawnPointManager spawnPointManager)
        {
            _signalBus = signalBus;
            _networkManager = networkManager;
            _playerPrefab = playerPrefab;
            _spawnPointManager = spawnPointManager;
        }

        // ==================================================
        // INITIALIZE
        // ==================================================

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

            if (_spawnPointManager == null)
            {
                Debug.LogError(
                    "[RESPAWN SERVICE] " +
                    "SpawnPointManager is NULL!"
                );

                return;
            }

            if (_spawnPointManager.Count == 0)
            {
                Debug.LogError(
                    "[RESPAWN SERVICE] " +
                    "No spawn points configured!"
                );

                return;
            }

            _signalBus.Subscribe<NetworkPlayerDeathSignal>(
                OnPlayerDeath
            );

            _isSubscribed = true;

            Debug.Log(
                "[RESPAWN SERVICE] " +
                "Subscribed to NetworkPlayerDeathSignal."
            );
        }

        // ==================================================
        // PLAYER DEATH
        // ==================================================

        private void OnPlayerDeath(
            NetworkPlayerDeathSignal signal)
        {
            if (!_networkManager.IsServer)
                return;

            ulong clientId =
                signal.ClientId;

            Debug.Log(
                $"[RESPAWN SERVICE] " +
                $"Death signal received | " +
                $"ClientId: {clientId}"
            );

            NetworkPlayer player =
                FindPlayer(clientId);

            if (player != null)
            {
                DespawnPlayer(player);
            }
            else
            {
                Debug.LogWarning(
                    $"[RESPAWN SERVICE] " +
                    $"Player not found | " +
                    $"ClientId: {clientId}"
                );
            }

            CoroutineRunner.Start(
                RespawnCoroutine(clientId)
            );
        }

        // ==================================================
        // FIND PLAYER
        // ==================================================

        private NetworkPlayer FindPlayer(
            ulong clientId)
        {
            if (!_networkManager.ConnectedClients.TryGetValue(
                    clientId,
                    out NetworkClient client))
            {
                return null;
            }

            if (client.PlayerObject == null)
                return null;

            return client.PlayerObject
                .GetComponent<NetworkPlayer>();
        }

        // ==================================================
        // DESPAWN
        // ==================================================

        private void DespawnPlayer(
            NetworkPlayer player)
        {
            NetworkObject networkObject =
                player.GetComponent<NetworkObject>();

            if (networkObject == null)
            {
                Debug.LogError(
                    "[RESPAWN SERVICE] " +
                    "NetworkObject missing on player!"
                );

                return;
            }

            if (!networkObject.IsSpawned)
            {
                Debug.LogWarning(
                    "[RESPAWN SERVICE] " +
                    "Player is already despawned."
                );

                return;
            }

            Debug.Log(
                $"[RESPAWN SERVICE] " +
                $"Despawning player | " +
                $"ClientId: {networkObject.OwnerClientId}"
            );

            networkObject.Despawn(true);
        }

        // ==================================================
        // RESPAWN COROUTINE
        // ==================================================

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

        // ==================================================
        // RESPAWN
        // ==================================================

        private void RespawnPlayer(
            ulong clientId)
        {
            if (!_networkManager.IsServer)
                return;

            if (_playerPrefab == null)
            {
                Debug.LogError(
                    "[RESPAWN SERVICE] " +
                    "Player prefab is NULL!"
                );

                return;
            }

            if (_spawnPointManager == null)
            {
                Debug.LogError(
                    "[RESPAWN SERVICE] " +
                    "SpawnPointManager is NULL!"
                );

                return;
            }

            Transform spawnPoint =
                _spawnPointManager.GetSpawnPoint(
                    clientId
                );

            if (spawnPoint == null)
            {
                Debug.LogError(
                    "[RESPAWN SERVICE] " +
                    "Spawn point could not be found!"
                );

                return;
            }

            Debug.Log(
                $"[RESPAWN SERVICE] " +
                $"Respawning ClientId: {clientId} | " +
                $"Position: {spawnPoint.position}"
            );

            NetworkPlayer player =
                UnityEngine.Object.Instantiate(
                    _playerPrefab,
                    spawnPoint.position,
                    spawnPoint.rotation
                );

            NetworkObject networkObject =
                player.GetComponent<NetworkObject>();

            if (networkObject == null)
            {
                Debug.LogError(
                    "[RESPAWN SERVICE] " +
                    "NetworkObject missing on player prefab!"
                );

                UnityEngine.Object.Destroy(
                    player.gameObject
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

        // ==================================================
        // DISPOSE
        // ==================================================

        public void Dispose()
        {
            if (!_isSubscribed)
                return;

            _signalBus.TryUnsubscribe<
                NetworkPlayerDeathSignal>(
                    OnPlayerDeath
                );

            _isSubscribed = false;

            Debug.Log(
                "[RESPAWN SERVICE] Disposed."
            );
        }

        // ==================================================
        // COROUTINE RUNNER
        // ==================================================

        private class CoroutineRunner :
            MonoBehaviour
        {
            private static CoroutineRunner _instance;

            public static Coroutine Start(
                IEnumerator routine)
            {
                if (_instance == null)
                {
                    GameObject obj =
                        new GameObject(
                            "[Network Respawn Coroutine Runner]"
                        );

                    UnityEngine.Object.DontDestroyOnLoad(
                        obj
                    );

                    _instance =
                        obj.AddComponent<
                            CoroutineRunner>();
                }

                return _instance.StartCoroutine(
                    routine
                );
            }
        }
    }
}
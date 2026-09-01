using System;
using System.Collections;
using FireLine.Scripts.Network.Signals;
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
            Debug.Log($"[RESPAWN SERVICE] Initialize | " + $"IsServer: {_networkManager.IsServer} | " + $"IsClient: {_networkManager.IsClient} | " + $"IsHost: {_networkManager.IsHost}");
            _coroutineRunner.Run(WaitForServerAndSubscribe()); 
        }

        private IEnumerator WaitForServerAndSubscribe() 
        { 
            Debug.Log("[RESPAWN SERVICE] " + "Waiting for NetworkManager...");
            while (_networkManager != null && !_networkManager.IsServer) 
            { 
                yield return null; 
            } 
            
            if (_networkManager == null) 
            { 
                Debug.LogError("[RESPAWN SERVICE] " + "NetworkManager is NULL!"); 
                yield break; 
            }

            Debug.Log($"[RESPAWN SERVICE] " + $"NetworkManager ready | " + $"IsServer: {_networkManager.IsServer} | " + $"IsHost: {_networkManager.IsHost}");
            _signalBus.Subscribe<NetworkPlayerDeathSignal>(OnPlayerDeath); Debug.Log("[RESPAWN SERVICE] " + "Subscribed to NetworkPlayerDeathSignal"); 
        }

        public void Dispose()
        {
            if (_signalBus == null)
                return;

            if (_networkManager == null)
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
                $"Victim: {signal.VictimClientId} | " +
                $"Killer: {signal.KillerClientId}"
            );

            _coroutineRunner.Run(
                RespawnCoroutine(
                    signal.VictimClientId
                )
            );
        }

        private IEnumerator RespawnCoroutine(
            ulong clientId)
        {
            Debug.Log(
                $"[RESPAWN SERVICE] Waiting {RespawnDelay}s | " +
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
                $"[RESPAWN SERVICE] RespawnPlayer | " +
                $"ClientId: {clientId}"
            );

            // --------------------------------------------------
            // 1. Client hâlâ bağlı mı?
            // --------------------------------------------------

            if (!_networkManager.ConnectedClients.TryGetValue(
                    clientId,
                    out NetworkClient client))
            {
                Debug.LogWarning(
                    $"[RESPAWN SERVICE] " +
                    $"Client disconnected | " +
                    $"ClientId: {clientId}"
                );

                return;
            }

            // --------------------------------------------------
            // 2. Eski Player'ı al
            // --------------------------------------------------

            NetworkObject oldPlayer =
                client.PlayerObject;

            if (oldPlayer != null)
            {
                Debug.Log(
                    $"[RESPAWN SERVICE] " +
                    $"Old Player found | " +
                    $"Spawned: {oldPlayer.IsSpawned} | " +
                    $"ClientId: {clientId}"
                );

                if (oldPlayer.IsSpawned)
                {
                    oldPlayer.Despawn(
                        true
                    );

                    Debug.Log(
                        $"[RESPAWN SERVICE] " +
                        $"Old Player despawned | " +
                        $"ClientId: {clientId}"
                    );
                }
            }
            else
            {
                Debug.LogWarning(
                    $"[RESPAWN SERVICE] " +
                    $"Old PlayerObject is NULL | " +
                    $"ClientId: {clientId}"
                );
            }

            // --------------------------------------------------
            // 3. Spawn point bul
            // --------------------------------------------------

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
                    $"[RESPAWN SERVICE] " +
                    $"SpawnPoint is NULL | " +
                    $"ClientId: {clientId}"
                );

                return;
            }

            Debug.Log(
                $"[RESPAWN SERVICE] " +
                $"Spawn point found | " +
                $"ClientId: {clientId} | " +
                $"Position: {spawnPoint.position}"
            );

            // --------------------------------------------------
            // 4. Player prefab bul
            // --------------------------------------------------

            GameObject playerPrefab =
                _networkManager
                    .NetworkConfig
                    .PlayerPrefab;

            if (playerPrefab == null)
            {
                Debug.LogError(
                    "[RESPAWN SERVICE] " +
                    "NetworkConfig.PlayerPrefab is NULL!"
                );

                return;
            }

            // --------------------------------------------------
            // 5. Yeni Player oluştur
            // --------------------------------------------------

            GameObject newPlayer =
                UnityEngine.Object.Instantiate(
                    playerPrefab,
                    spawnPoint.position,
                    spawnPoint.rotation
                );

            if (newPlayer == null)
            {
                Debug.LogError(
                    "[RESPAWN SERVICE] " +
                    "Failed to instantiate PlayerPrefab!"
                );

                return;
            }

            Debug.Log(
                $"[RESPAWN SERVICE] " +
                $"New Player instantiated | " +
                $"ClientId: {clientId}"
            );

            // --------------------------------------------------
            // 6. NetworkObject kontrolü
            // --------------------------------------------------

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

            // --------------------------------------------------
            // 7. Network Player olarak spawn et
            // --------------------------------------------------

            networkObject.SpawnAsPlayerObject(
                clientId,
                true
            );

            Debug.Log(
                $"[RESPAWN SERVICE] " +
                $"Player respawned successfully | " +
                $"ClientId: {clientId} | " +
                $"Position: {spawnPoint.position}"
            );
        }
    }
}
using System.Collections;
using FireLine.Scripts.Network.Signals;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace FireLine.Scripts.Network.Service
{
    public class NetworkPlayerRespawnService : IInitializable, ITickable
    {
        private readonly SignalBus _signalBus;
        private readonly NetworkManager _networkManager;
        private readonly DiContainer _container;

        private Transform[] _spawnPoints;

        private bool _isSubscribed;

        private const float RespawnDelay = 2f;

        public NetworkPlayerRespawnService(
            SignalBus signalBus,
            NetworkManager networkManager,
            DiContainer container)
        {
            _signalBus = signalBus;
            _networkManager = networkManager;
            _container = container;
        }

        public void Initialize()
        {
            Debug.Log(
                "[RESPAWN SERVICE] Initialize"
            );

            if (_networkManager == null)
            {
                Debug.LogError(
                    "[RESPAWN SERVICE] NetworkManager is NULL!"
                );

                return;
            }

            _networkManager.OnServerStarted +=
                HandleServerStarted;

            if (_networkManager.IsServer)
            {
                HandleServerStarted();
            }
        }

        private void HandleServerStarted()
        {
            if (_isSubscribed)
                return;

            Debug.Log(
                "[RESPAWN SERVICE] Server started."
            );

            _signalBus.Subscribe<NetworkPlayerDeathSignal>(
                OnPlayerDeath
            );

            _isSubscribed = true;

            Debug.Log(
                "[RESPAWN SERVICE] Successfully subscribed."
            );
        }

        private void OnPlayerDeath(
            NetworkPlayerDeathSignal signal)
        {
            if (!_networkManager.IsServer)
                return;

            Debug.Log(
                $"[RESPAWN SERVICE] SIGNAL RECEIVED | " +
                $"ClientId: {signal.ClientId}"
            );

            CoroutineRunner.Instance.StartCoroutine(
                RespawnCoroutine(signal.ClientId)
            );
        }

        private IEnumerator RespawnCoroutine(
            ulong clientId)
        {
            Debug.Log(
                $"[RESPAWN] Waiting {RespawnDelay} seconds..."
            );

            yield return new WaitForSeconds(
                RespawnDelay
            );

            RespawnPlayer(clientId);
        }

        private void RespawnPlayer(ulong clientId)
        {
            if (!_networkManager.IsServer)
                return;

            if (!_networkManager.ConnectedClients.TryGetValue(
                    clientId,
                    out NetworkClient client))
            {
                Debug.LogError(
                    $"[RESPAWN] Client {clientId} not found."
                );

                return;
            }

            NetworkObject oldPlayer =
                client.PlayerObject;

            if (oldPlayer == null)
            {
                Debug.LogError(
                    "[RESPAWN] Old PlayerObject is NULL."
                );

                return;
            }

            Vector3 spawnPosition =
                oldPlayer.transform.position;

            Debug.Log(
                $"[RESPAWN] Destroying old Player | " +
                $"ObjectId: {oldPlayer.NetworkObjectId}"
            );

            oldPlayer.Despawn(true);

            Debug.Log(
                "[RESPAWN] Old Player destroyed."
            );

            GameObject playerPrefab =
                _networkManager
                    .NetworkConfig
                    .PlayerPrefab;

            if (playerPrefab == null)
            {
                Debug.LogError(
                    "[RESPAWN] PlayerPrefab is NULL!"
                );

                return;
            }

            GameObject newPlayer =
                Object.Instantiate(
                    playerPrefab,
                    spawnPosition,
                    Quaternion.identity
                );

            NetworkObject networkObject =
                newPlayer.GetComponent<NetworkObject>();

            if (networkObject == null)
            {
                Debug.LogError(
                    "[RESPAWN] NetworkObject missing!"
                );

                Object.Destroy(newPlayer);

                return;
            }

            networkObject.SpawnAsPlayerObject(
                clientId,
                true
            );

            Debug.Log(
                $"[RESPAWN] New Player spawned | " +
                $"ObjectId: {networkObject.NetworkObjectId}"
            );
        }

        public void Tick()
        {
        }

        public void Dispose()
        {
            if (_networkManager != null)
            {
                _networkManager.OnServerStarted -=
                    HandleServerStarted;
            }

            if (_isSubscribed)
            {
                _signalBus.TryUnsubscribe<NetworkPlayerDeathSignal>(
                    OnPlayerDeath
                );

                _isSubscribed = false;
            }
        }
    }
}
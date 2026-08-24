using System.Collections;
using FireLine.Scripts.Network.Signals;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace FireLine.Scripts.Network.Controller
{
    public class NetworkPlayerRespawnController :
        MonoBehaviour
    {
        [SerializeField]
        private float respawnDelay = 2f;

        private SignalBus _signalBus;
        private NetworkManager _networkManager;
        private NetworkSpawnPointManager _spawnPointManager;

        private bool _isSubscribed;

        [Inject]
        public void Construct(
            SignalBus signalBus,
            NetworkManager networkManager,
            NetworkSpawnPointManager spawnPointManager)
        {
            _signalBus = signalBus;
            _networkManager = networkManager;
            _spawnPointManager = spawnPointManager;
        }

        private void Start()
        {
            if (_networkManager == null)
            {
                Debug.LogError(
                    "[RESPAWN] NetworkManager is NULL!"
                );

                return;
            }

            if (!_networkManager.IsServer)
                return;

            if (_signalBus == null)
            {
                Debug.LogError(
                    "[RESPAWN] SignalBus is NULL!"
                );

                return;
            }

            _signalBus.Subscribe<NetworkPlayerDeathSignal>(
                OnPlayerDeath
            );

            _isSubscribed = true;

            Debug.Log(
                "[RESPAWN] Successfully subscribed."
            );
        }

        private void OnDestroy()
        {
            if (!_isSubscribed)
                return;

            if (_signalBus == null)
                return;

            _signalBus.Unsubscribe<NetworkPlayerDeathSignal>(
                OnPlayerDeath
            );

            _isSubscribed = false;
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
            if (!_networkManager.ConnectedClients.TryGetValue(
                    clientId,
                    out NetworkClient client))
            {
                Debug.LogError(
                    $"[RESPAWN] Client not found: {clientId}"
                );

                yield break;
            }

            if (client.PlayerObject != null)
            {
                client.PlayerObject.Despawn();

                Debug.Log(
                    $"[RESPAWN] Player despawned | " +
                    $"ClientId: {clientId}"
                );
            }

            yield return new WaitForSeconds(
                respawnDelay
            );

            if (!_networkManager.ConnectedClients.ContainsKey(
                    clientId))
            {
                Debug.LogWarning(
                    $"[RESPAWN] Client disconnected: {clientId}"
                );

                yield break;
            }

            Transform spawnPoint =
                _spawnPointManager.GetNextSpawnPoint();

            if (spawnPoint == null)
            {
                Debug.LogError(
                    "[RESPAWN] SpawnPoint is NULL!"
                );

                yield break;
            }

            GameObject playerPrefab =
                _networkManager
                    .NetworkConfig
                    .PlayerPrefab;

            if (playerPrefab == null)
            {
                Debug.LogError(
                    "[RESPAWN] PlayerPrefab is NULL!"
                );

                yield break;
            }

            GameObject player =
                Instantiate(
                    playerPrefab,
                    spawnPoint.position,
                    spawnPoint.rotation
                );

            NetworkObject networkObject =
                player.GetComponent<NetworkObject>();

            if (networkObject == null)
            {
                Debug.LogError(
                    "[RESPAWN] PlayerPrefab has no NetworkObject!"
                );

                Destroy(player);

                yield break;
            }

            networkObject.SpawnAsPlayerObject(
                clientId,
                true
            );

            Debug.Log(
                $"[RESPAWN] Player respawned | " +
                $"ClientId: {clientId} | " +
                $"Position: {spawnPoint.position}"
            );
        }
    }
}
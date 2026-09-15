using Unity.Netcode;
using UnityEngine;

namespace FireLine.Scripts.Network
{
    public class GamePlayerSpawner : MonoBehaviour
    {
        [Header("Player")]
        [SerializeField]
        private NetworkObject playerPrefab;

        [Header("Spawn Points")]
        [SerializeField]
        private Transform[] spawnPoints;

        private NetworkManager _networkManager;

        private void Awake()
        {
            _networkManager =
                NetworkManager.Singleton;

            if (_networkManager == null)
            {
                Debug.LogError(
                    "[PLAYER SPAWNER] " +
                    "NetworkManager.Singleton is NULL!"
                );

                return;
            }

            if (!_networkManager.IsServer)
            {
                Debug.Log(
                    "[PLAYER SPAWNER] " +
                    "Not server. Spawner disabled."
                );

                return;
            }

            if (playerPrefab == null)
            {
                Debug.LogError(
                    "[PLAYER SPAWNER] " +
                    "Player Prefab is NULL!"
                );
            }

            _networkManager
                .OnClientConnectedCallback +=
                OnClientConnected;

            Debug.Log(
                "[PLAYER SPAWNER] " +
                "Server spawner initialized."
            );
        }

        private void Start()
        {
            if (_networkManager == null)
                return;

            if (!_networkManager.IsServer)
                return;

            Debug.Log(
                $"[PLAYER SPAWNER] " +
                $"Game scene ready | " +
                $"ConnectedClients=" +
                $"{_networkManager.ConnectedClientsIds.Count}"
            );

            SpawnExistingPlayers();
        }

        private void OnClientConnected(
            ulong clientId)
        {
            if (_networkManager == null)
                return;

            if (!_networkManager.IsServer)
                return;

            Debug.Log(
                $"[PLAYER SPAWNER] " +
                $"Client connected | " +
                $"ClientId={clientId} | " +
                $"Scene={gameObject.scene.name}"
            );

            SpawnPlayer(clientId);
        }

        private void SpawnExistingPlayers()
        {
            if (_networkManager == null)
                return;

            Debug.Log(
                "[PLAYER SPAWNER] " +
                "Checking existing connected players..."
            );

            foreach (ulong clientId in
                     _networkManager.ConnectedClientsIds)
            {
                SpawnPlayer(clientId);
            }
        }

        private void SpawnPlayer(
            ulong clientId)
        {
            if (_networkManager == null)
                return;

            if (!_networkManager.IsServer)
                return;

            if (!_networkManager.ConnectedClients.TryGetValue(
                    clientId,
                    out NetworkClient client))
            {
                Debug.LogWarning(
                    $"[PLAYER SPAWNER] " +
                    $"Client not found | " +
                    $"ClientId={clientId}"
                );

                return;
            }

            if (client.PlayerObject != null)
            {
                Debug.Log(
                    $"[PLAYER SPAWNER] " +
                    $"Player already exists | " +
                    $"ClientId={clientId} | " +
                    $"Player={client.PlayerObject.name}"
                );

                return;
            }

            if (playerPrefab == null)
            {
                Debug.LogError(
                    "[PLAYER SPAWNER] " +
                    "Cannot spawn player. " +
                    "Player Prefab is NULL!"
                );

                return;
            }

            Transform spawnPoint =
                GetSpawnPoint(clientId);

            if (spawnPoint == null)
            {
                Debug.LogError(
                    $"[PLAYER SPAWNER] " +
                    $"SpawnPoint is NULL | " +
                    $"ClientId={clientId}"
                );

                return;
            }

            Debug.Log(
                $"[PLAYER SPAWNER] " +
                $"SPAWN POINT TEST | " +
                $"ClientId={clientId} | " +
                $"Point={spawnPoint.name} | " +
                $"Position={spawnPoint.position}"
            );

            NetworkObject player =
                Instantiate(
                    playerPrefab,
                    spawnPoint.position,
                    spawnPoint.rotation
                );

            if (player == null)
            {
                Debug.LogError(
                    $"[PLAYER SPAWNER] " +
                    $"Failed to instantiate Player | " +
                    $"ClientId={clientId}"
                );

                return;
            }

            player.SpawnAsPlayerObject(
                clientId,
                true
            );

            Debug.Log(
                $"[PLAYER SPAWNER] " +
                $"Spawned Player | " +
                $"ClientId={clientId} | " +
                $"Player={player.name} | " +
                $"NetworkObjectId={player.NetworkObjectId} | " +
                $"SpawnPoint={spawnPoint.name} | " +
                $"Position={spawnPoint.position}"
            );
        }

        private Transform GetSpawnPoint(
            ulong clientId)
        {
            if (spawnPoints == null ||
                spawnPoints.Length == 0)
            {
                Debug.LogError(
                    "[PLAYER SPAWNER] " +
                    "No spawn points assigned!"
                );

                return null;
            }

            int index =
                (int)(
                    clientId %
                    (ulong)spawnPoints.Length
                );

            return spawnPoints[index];
        }

        private void OnDestroy()
        {
            if (_networkManager == null)
                return;

            _networkManager
                .OnClientConnectedCallback -=
                OnClientConnected;
        }
    }
}
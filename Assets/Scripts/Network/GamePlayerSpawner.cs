using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FireLine.Scripts.Network
{
    public class GamePlayerSpawner : MonoBehaviour
    {
        [SerializeField]
        private NetworkObject playerPrefab;

        [SerializeField]
        private Transform[] spawnPoints;

        private void Awake()
        {
            if (!NetworkManager.Singleton.IsServer)
                return;

            NetworkManager.Singleton
                .OnClientConnectedCallback +=
                OnClientConnected;
        }

        private void Start()
        {
            if (!NetworkManager.Singleton.IsServer)
                return;

            SpawnExistingPlayers();
        }

        private void OnClientConnected(
            ulong clientId)
        {
            if (SceneManager.GetActiveScene().name != "Game")
                return;

            SpawnPlayer(clientId);
        }

        private void SpawnExistingPlayers()
        {
            foreach (ulong clientId in
                     NetworkManager.Singleton.ConnectedClientsIds)
            {
                SpawnPlayer(clientId);
            }
        }

        private void SpawnPlayer(
            ulong clientId)
        {
            if (!NetworkManager.Singleton
                .ConnectedClients
                .TryGetValue(
                    clientId,
                    out NetworkClient client))
            {
                return;
            }

            if (client.PlayerObject != null)
                return;

            Transform spawnPoint =
                GetSpawnPoint(clientId);

            Debug.Log(
                $"[PLAYER SPAWNER] SPAWN POINT TEST | " +
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

            player.SpawnAsPlayerObject(
                clientId,
                true
            );

            Debug.Log(
                $"[PLAYER SPAWNER] " +
                $"Spawned Player | " +
                $"ClientId={clientId} | " +
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

                return transform;
            }

            int index =
                (int)(clientId % (ulong)spawnPoints.Length);

            return spawnPoints[index];
        }

        private void OnDestroy()
        {
            if (NetworkManager.Singleton == null)
                return;

            NetworkManager.Singleton
                .OnClientConnectedCallback -=
                OnClientConnected;
        }
    }
}
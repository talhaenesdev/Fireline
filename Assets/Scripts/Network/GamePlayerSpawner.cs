using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FireLine.Scripts.Network
{
    public class GamePlayerSpawner : MonoBehaviour
    {
        [SerializeField]
        private NetworkObject playerPrefab;

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

            NetworkObject player =
                Instantiate(
                    playerPrefab,
                    Vector3.zero,
                    Quaternion.identity
                );

            player.SpawnAsPlayerObject(
                clientId,
                true
            );

            Debug.Log(
                $"[PLAYER SPAWNER] " +
                $"Spawned Player | " +
                $"ClientId={clientId}"
            );
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
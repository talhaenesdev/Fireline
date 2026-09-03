using FireLine.Scripts.Network.Service;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace FireLine.Scripts.Network.Installers
{
    public class NetworkInstaller : MonoInstaller
    {
        [Header("Player")]
        [SerializeField]
        private NetworkObject playerPrefab;

        public override void InstallBindings()
        {
            Debug.Log(
                $"[NETWORK INSTALLER] " +
                $"Container={Container.GetHashCode()}"
            );

            // --------------------------------------------------
            // NetworkManager
            // --------------------------------------------------

            Container.Bind<NetworkManager>()
                .FromMethod(_ => NetworkManager.Singleton)
                .AsSingle();

            // --------------------------------------------------
            // Player Prefab
            // --------------------------------------------------

            if (playerPrefab == null)
            {
                Debug.LogError(
                    "[NETWORK INSTALLER] " +
                    "Player Prefab is NULL!"
                );
            }
            else
            {
                Container.Bind<NetworkObject>()
                    .WithId("PlayerPrefab")
                    .FromInstance(playerPrefab)
                    .AsSingle();

                Debug.Log(
                    $"[NETWORK INSTALLER] " +
                    $"Player Prefab BOUND | " +
                    $"Prefab: {playerPrefab.name}"
                );
            }

            // --------------------------------------------------
            // Coroutine Runner
            // --------------------------------------------------

            Container.Bind<NetworkCoroutineRunner>()
                .FromNewComponentOnNewGameObject()
                .AsSingle()
                .NonLazy();

            // --------------------------------------------------
            // Services
            // --------------------------------------------------

            Container.BindInterfacesTo<NetworkPlayerRespawnService>()
                .AsSingle();

            Container.BindInterfacesAndSelfTo<NetworkPlayerScoreService>()
                .AsSingle();

            Container.Bind<NetworkConnectionService>()
                .AsSingle();

            // --------------------------------------------------
            // Spawn Point Manager
            // --------------------------------------------------

            NetworkSpawnPointManager spawnPointManager =
                FindFirstObjectByType<NetworkSpawnPointManager>();

            if (spawnPointManager == null)
            {
                Debug.LogError(
                    "[NETWORK INSTALLER] " +
                    "spawnPointManager NOT FOUND!"
                );
            }
            else
            {
                Container.Bind<NetworkSpawnPointManager>()
                    .FromInstance(spawnPointManager)
                    .AsSingle();

                Debug.Log(
                    "[NETWORK INSTALLER] " +
                    "spawnPointManager BOUND"
                );
            }

            // --------------------------------------------------
            // Bullet Spawner
            // --------------------------------------------------

            NetworkBulletSpawner bulletSpawner =
                FindFirstObjectByType<NetworkBulletSpawner>();

            if (bulletSpawner == null)
            {
                Debug.LogError(
                    "[NETWORK INSTALLER] " +
                    "NetworkBulletSpawner NOT FOUND!"
                );
            }
            else
            {
                Container.Bind<NetworkBulletSpawner>()
                    .FromInstance(bulletSpawner)
                    .AsSingle();

                Debug.Log(
                    "[NETWORK INSTALLER] " +
                    "NetworkBulletSpawner BOUND"
                );
            }

            // --------------------------------------------------
            // Scoreboard
            // --------------------------------------------------

            NetworkScoreboard scoreboard =
                FindFirstObjectByType<NetworkScoreboard>();

            if (scoreboard == null)
            {
                Debug.LogError(
                    "[NETWORK INSTALLER] " +
                    "scoreboard NOT FOUND!"
                );
            }
            else
            {
                Container.Bind<NetworkScoreboard>()
                    .FromInstance(scoreboard)
                    .AsSingle();

                Debug.Log(
                    "[NETWORK INSTALLER] " +
                    "scoreboard BOUND"
                );
            }

            Debug.Log(
                $"[NETWORK INSTALLER] " +
                $"BulletSpawner found: " +
                $"{FindFirstObjectByType<NetworkBulletSpawner>() != null}"
            );

            Debug.Log(
                "[NETWORK INSTALLER] COMPLETED"
            );
        }
    }
}
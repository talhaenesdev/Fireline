using FireLine.Scripts.Network.Service;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace FireLine.Scripts.Network.Installers
{
    public class NetworkInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Debug.Log(
                $"[NETWORK INSTALLER] " +
                $"Container={Container.GetHashCode()}"
            );

            Container.Bind<NetworkManager>()
                .FromMethod(_ => NetworkManager.Singleton)
                .AsSingle();

            Container.Bind<NetworkCoroutineRunner>()
                .FromNewComponentOnNewGameObject()
                .AsSingle()
                .NonLazy();

            Container.BindInterfacesTo<NetworkPlayerRespawnService>()
                .AsSingle();

            Container.BindInterfacesTo<NetworkPlayerScoreService>()
                .AsSingle();

            Container.Bind<NetworkConnectionService>()
                .AsSingle();





            NetworkSpawnPointManager spawnPointManager =
                FindFirstObjectByType<NetworkSpawnPointManager>();

            if (spawnPointManager == null)
            {
                Debug.LogError(
                    "[NETWORK INSTALLER] spawnPointManager NOT FOUND!"
                );
            }
            else
            {
                Container.Bind<NetworkSpawnPointManager>()
                    .FromInstance(spawnPointManager)
                    .AsSingle();

                Debug.Log(
                    "[NETWORK INSTALLER] spawnPointManager BOUND"
                );
            }








            NetworkBulletSpawner bulletSpawner =
                FindFirstObjectByType<NetworkBulletSpawner>();

            if (bulletSpawner == null)
            {
                Debug.LogError(
                    "[NETWORK INSTALLER] NetworkBulletSpawner NOT FOUND!"
                );
            }
            else
            {
                Container.Bind<NetworkBulletSpawner>()
                    .FromInstance(bulletSpawner)
                    .AsSingle();

                Debug.Log(
                    "[NETWORK INSTALLER] NetworkBulletSpawner BOUND"
                );
            }

            NetworkScoreboard scoreboard =
                FindFirstObjectByType<NetworkScoreboard>();

            if (scoreboard == null)
            {
                Debug.LogError(
                    "[NETWORK INSTALLER] scoreboard NOT FOUND!"
                );
            }
            else
            {
                Container.Bind<NetworkScoreboard>()
                    .FromInstance(scoreboard)
                    .AsSingle();

                Debug.Log(
                    "[NETWORK INSTALLER] scoreboard BOUND"
                );
            }


            Debug.Log(
                $"[NETWORK INSTALLER] " +
                $"BulletSpawner found: " +
                $"{FindFirstObjectByType<NetworkBulletSpawner>() != null}"
            );

            Debug.Log("[NETWORK INSTALLER] COMPLETED");
        }
    }
}
using FireLine.Scripts.Network;
using FireLine.Scripts.Network.Service;
using FireLine.Scripts.Network.Signals;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace FireLine.Scripts.Installers
{
    public class NetworkInstaller : MonoInstaller
    {
        [SerializeField]
        private NetworkPlayer playerPrefab;

        public override void InstallBindings()
        {
            // Signals
            Container.DeclareSignal<
                NetworkPlayerDeathSignal>();

            // Network Manager
            Container.Bind<NetworkManager>()
                .FromComponentInHierarchy()
                .AsSingle();

            // Spawn Point Manager
            Container.Bind<NetworkSpawnPointManager>()
                .FromComponentInHierarchy()
                .AsSingle();

            // Player Prefab
            Container.Bind<NetworkPlayer>()
                .FromInstance(playerPrefab)
                .AsSingle();

            // Respawn Service
            Container.BindInterfacesAndSelfTo<
                NetworkPlayerRespawnService>()
                .AsSingle();
        }
    }
}
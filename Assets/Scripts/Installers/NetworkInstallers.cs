using FireLine.Scripts.Network;
using FireLine.Scripts.Network.Service;
using FireLine.Scripts.Network.Signals;
using Unity.Netcode;
using Zenject;

namespace FireLine.Scripts.Installers
{
    public class NetworkInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            // ----------------------------------------
            // SIGNALS
            // ----------------------------------------

            Container.DeclareSignal<
                NetworkPlayerDeathSignal>();

            Container.DeclareSignal<
                NetworkShootSignal>();


            // ----------------------------------------
            // NETWORK RESPAWN
            // ----------------------------------------

            Container.BindInterfacesTo<
                NetworkPlayerRespawnService>()
                .AsSingle();

            Container.Bind<NetworkCoroutineRunner>()
                .FromNewComponentOnNewGameObject()
                .AsSingle()
                .NonLazy();


            // ----------------------------------------
            // NETWORK SERVICES
            // ----------------------------------------

            Container.Bind<NetworkSpawnPointManager>()
                .FromComponentInHierarchy()
                .AsSingle();

            Container.Bind<NetworkManager>()
                .FromComponentInHierarchy()
                .AsSingle();

            Container.Bind<NetworkBulletSpawner>()
                .FromComponentInHierarchy()
                .AsSingle();
        }
    }
}
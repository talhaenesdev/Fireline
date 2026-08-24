using FireLine.Scripts.Network;
using FireLine.Scripts.Network.Controller;
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
            Container.DeclareSignal<NetworkPlayerDeathSignal>();

            Container.BindInterfacesTo<NetworkPlayerRespawnService>()
                .AsSingle();
            
            Container.Bind<NetworkSpawnPointManager>()
                .FromComponentInHierarchy()
                .AsSingle();

            Container.Bind<NetworkManager>()
                .FromComponentInHierarchy()
                .AsSingle();

            Container.Bind<NetworkPlayerRespawnController>()
                .FromComponentInHierarchy()
                .AsSingle();
        }
    }
}
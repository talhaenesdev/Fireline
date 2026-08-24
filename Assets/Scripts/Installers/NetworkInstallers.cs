using FireLine.Scripts.Core.Weapon;
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
            Container.DeclareSignal<
                NetworkPlayerDeathSignal>();

            Container.DeclareSignal<
                NetworkShootSignal>();

            Container.BindInterfacesTo<
                NetworkPlayerRespawnService>()
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

            Container.Bind<NetworkBulletSpawner>()
                .FromComponentInHierarchy()
                .AsSingle();

            Container.Bind<IWeaponFireService>()
    .To<NetworkWeaponFireService>()
    .FromComponentInHierarchy()
    .AsSingle();

            Container.Bind<NetworkCoroutineRunner>()
    .FromNewComponentOnNewGameObject()
    .AsSingle()
    .NonLazy();
        }
    }
}
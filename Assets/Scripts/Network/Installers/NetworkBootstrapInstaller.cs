using FireLine.Scripts.Network.Service;
using Unity.Netcode;
using Zenject;
using UnityEngine;

namespace FireLine.Scripts.Network.Installers
{
    public class NetworkBootstrapInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Debug.Log(
        $"[NETWORK BOOTSTRAP] " +
        $"Scene: {gameObject.scene.name} | " +
        $"Container: {Container.GetHashCode()}"
    );
            Container.Bind<NetworkManager>()
                .FromComponentInHierarchy()
                .AsSingle();

            Container.Bind<NetworkGameStartService>()
                .AsSingle();

            Container.Bind<NetworkLobbyService>()
                .AsSingle();
        }
    }
}
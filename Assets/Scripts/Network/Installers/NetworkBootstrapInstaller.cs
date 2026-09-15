using FireLine.Scripts.Core.Scene.Model;
using FireLine.Scripts.Network.Service;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace FireLine.Scripts.Network.Installers
{
    public class NetworkBootstrapInstaller : MonoInstaller
    {
        [Header("Scenes")]
        [SerializeField]
        private SceneReference gameScene;

        public override void InstallBindings()
        {
            Debug.Log(
                $"[NETWORK BOOTSTRAP] " +
                $"Scene: {gameObject.scene.name} | " +
                $"Container: {Container.GetHashCode()}"
            );

            // --------------------------------------------------
            // NetworkManager
            // --------------------------------------------------

            Container.Bind<NetworkManager>()
                .FromComponentInHierarchy()
                .AsSingle();

            // --------------------------------------------------
            // Scene References
            // --------------------------------------------------

            if (gameScene == null)
            {
                Debug.LogError(
                    "[NETWORK BOOTSTRAP] " +
                    "Game SceneReference is NULL!"
                );
            }
            else
            {
                Container.BindInstance(gameScene)
                    .WithId("GameScene");

                Debug.Log(
                    $"[NETWORK BOOTSTRAP] " +
                    $"Game Scene bound | " +
                    $"Scene={gameScene.SceneName}"
                );
            }

            // --------------------------------------------------
            // Services
            // --------------------------------------------------

            Container.Bind<NetworkGameStartService>()
                .AsSingle();

            Container.Bind<NetworkLobbyService>()
                .AsSingle();
        }
    }
}
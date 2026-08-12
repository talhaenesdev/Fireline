using Zenject;
using FireLine.Scripts.Player.Model;
using FireLine.Scripts.Player.View;
using FireLine.Scripts.Player.Controller;

namespace FireLine.Scripts.Player.Installers
{
    public class PlayerInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<PlayerModel>()
                .AsSingle()
                .WithArguments(5f);

            Container.Bind<PlayerView>()
                .FromComponentInHierarchy()
                .AsSingle();

            Container.Bind<PlayerController>()
                .AsSingle();

            Container.BindInterfacesTo<PlayerInputController>()
                .AsSingle();
        }
    }
}
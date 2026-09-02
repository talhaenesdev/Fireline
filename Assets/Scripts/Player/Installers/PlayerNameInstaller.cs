using FireLine.Scripts.Player.Service;
using Zenject;

namespace Assets.Scripts.Player.Installers
{
    internal class PlayerNameInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container
                .Bind<PlayerNameService>()
                .AsSingle()
                .NonLazy();
        }
    }
}

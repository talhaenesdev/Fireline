using FireLine.Scripts.Core.Pooling;
using FireLine.Scripts.Player.Model;
using Zenject;

namespace FireLine.Scripts.Core.Installers
{
    public class ProjectInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<PlayerModel>()
                .AsSingle()
                .WithArguments(5f);

            Container.Bind<IPoolService>()
                .To<PoolService>()
                .AsSingle();
        }
    }
}
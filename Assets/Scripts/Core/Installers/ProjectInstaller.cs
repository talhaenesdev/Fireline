using FireLine.Scripts.Core.Services;
using FireLine.Scripts.Core.Services.Entities;
using FireLine.Scripts.Core.Signals;
using Zenject;

namespace FireLine.Scripts.Core.Installers
{
    public class ProjectInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {

            Container.Bind<IEntityDeathService>()
                .To<EntityDeathService>()
                .AsSingle();

            Container.BindInterfacesTo<EntityLifecycleService>()
                .AsSingle()
                .NonLazy();
        }
    }
}
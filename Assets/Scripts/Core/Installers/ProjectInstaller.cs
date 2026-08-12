
using FireLine.Scripts.Core.Signals;
using Zenject;

namespace FireLine.Scripts.Core.Installers
{
    public class ProjectInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.DeclareSignal<EntityDestroyedSignal>();
        }
    }
}
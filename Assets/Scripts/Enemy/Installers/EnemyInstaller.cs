using FireLine.Scripts.Enemy.Factory;
using FireLine.Scripts.Enemy.Spawner;
using System.ComponentModel;
using Zenject;

namespace FireLine.Scripts.Enemy.Installers
{
    public class EnemyInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<IEnemyFactory>()
                .To<EnemyFactory>()
                .AsSingle();

            Container.Bind<EnemySpawner>()
                .AsSingle();
        }
    }
}
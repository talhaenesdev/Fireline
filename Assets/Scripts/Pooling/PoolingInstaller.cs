using UnityEngine;
using Zenject;

namespace FireLine.Scripts.Pooling
{
    public class PoolingInstaller : MonoInstaller
    {
        [SerializeField]
        private PoolConfig poolConfig;

        public override void InstallBindings()
        {
            Container.Bind<IPoolService>()
                .To<PoolService>()
                .AsSingle()
                .WithArguments(poolConfig);
        }
    }
}
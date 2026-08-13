using FireLine.Scripts.Core.Services;
using FireLine.Scripts.Core.Services.Entities;
using FireLine.Scripts.Pooling;

namespace FireLine.Scripts.Services.Entities
{
    public class PoolEntityLifecycleHandler : IEntityLifecycleHandler
    {
        private readonly IPoolService _poolService;

        public PoolEntityLifecycleHandler(IPoolService poolService)
        {
            _poolService = poolService;
        }

        public bool CanHandle(Entity entity)
        {
            return entity is IPoolable &&
                   entity is IPoolKeyProvider;
        }

        public void Handle(Entity entity)
        {
            if (!(entity is IPoolKeyProvider poolKeyProvider))
                return;

            _poolService.Despawn(
                poolKeyProvider.PoolKey,
                entity
            );
        }
    }
}
using FireLine.Scripts.Core.Signals;
using Zenject;

namespace FireLine.Scripts.Core.Services.Entities
{
    public class EntityDeathService : IEntityDeathService
    {
        private readonly SignalBus _signalBus;

        public EntityDeathService(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }

        public void HandleDeath(Entity entity)
        {
            _signalBus.Fire(
                new EntityDestroyedSignal(entity)
            );
        }
    }
}
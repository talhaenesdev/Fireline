
using FireLine.Scripts.Core.Signals;
using Zenject;

namespace FireLine.Scripts.Core.Services.Entities
{
    public class EntityLifecycleService : IInitializable
    {
        private readonly SignalBus _signalBus;

        public EntityLifecycleService(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }

        public void Initialize()
        {
            _signalBus.Subscribe<EntityDestroyedSignal>(OnEntityDestroyed);
        }

        private void OnEntityDestroyed(EntityDestroyedSignal signal)
        {
            if (signal.Entity is not Core.Entities.Entity entity)
                return;

            HandleEntity(entity);
        }

        private void HandleEntity(Core.Entities.Entity entity)
        {
            // Þimdilik sadece test ediyoruz.
            UnityEngine.Debug.Log(
                $"EntityLifecycleService handled: {entity.name}"
            );
        }
    }
}
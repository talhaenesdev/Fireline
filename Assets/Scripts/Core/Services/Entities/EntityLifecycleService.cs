using FireLine.Scripts.Core.Signals;
using System.Collections.Generic;
using Zenject;

namespace FireLine.Scripts.Core.Services.Entities
{
    public class EntityLifecycleService : IInitializable
    {
        private readonly SignalBus _signalBus;
        private readonly List<IEntityLifecycleHandler> _handlers;

        public EntityLifecycleService(
            SignalBus signalBus,
            List<IEntityLifecycleHandler> handlers)
        {
            _signalBus = signalBus;
            _handlers = handlers;
        }

        public void Initialize()
        {
            _signalBus.Subscribe<EntityDestroyedSignal>(
                OnEntityDestroyed);
        }

        private void OnEntityDestroyed(EntityDestroyedSignal signal)
        {
            if (!(signal.Entity is Entity entity))
                return;

            foreach (var handler in _handlers)
            {
                if (!handler.CanHandle(entity))
                    continue;

                handler.Handle(entity);
                return;
            }
        }
    }
}
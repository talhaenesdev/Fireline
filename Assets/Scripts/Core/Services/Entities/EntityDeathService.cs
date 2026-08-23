using FireLine.Scripts.Core.Signals;
using UnityEngine;
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
            Debug.Log(
                $"[DEATH SERVICE] HandleDeath called | " +
                $"Entity: {entity.name}"
            );

            _signalBus.Fire(
                new EntityDestroyedSignal(entity)
            );

            Debug.Log(
                "[DEATH SERVICE] EntityDestroyedSignal fired."
            );
        }
    }
}
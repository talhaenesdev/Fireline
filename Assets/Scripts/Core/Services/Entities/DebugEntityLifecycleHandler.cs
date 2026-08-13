using FireLine.Scripts.Core.Services.Entities;
using UnityEngine;

namespace FireLine.Scripts.Services.Entities
{
    public class DebugEntityLifecycleHandler : IEntityLifecycleHandler
    {
        public bool CanHandle(Entity entity)
        {
            return true;
        }

        public void Handle(Entity entity)
        {
            Debug.Log(
                $"Lifecycle Handler handled: {entity.name}"
            );
        }
    }
}
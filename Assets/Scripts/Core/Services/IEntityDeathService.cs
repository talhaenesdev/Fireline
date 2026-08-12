
using FireLine.Scripts.Core.Entities;

namespace FireLine.Scripts.Core.Services
{
    public interface IEntityDeathService
    {
        void HandleDeath(Entity entity);
    }
}
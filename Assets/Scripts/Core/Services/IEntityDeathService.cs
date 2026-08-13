
using FireLine.Scripts.Core.Services.Entities;

namespace FireLine.Scripts.Core.Services
{
    public interface IEntityDeathService
    {
        void HandleDeath(Entity entity);
    }
}
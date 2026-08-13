

namespace FireLine.Scripts.Core.Services.Entities
{
    public interface IEntityLifecycleHandler
    {
        bool CanHandle(Entity entity);
        void Handle(Entity entity);
    }
}
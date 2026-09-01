using FireLine.Scripts.Core.Services.Entities;

namespace FireLine.Scripts.Core.Combat
{
    public interface IDamageSource
    {
        Entity Owner { get; }
    }
}
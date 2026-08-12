using UnityEngine;

namespace FireLine.Scripts.Core.Pooling
{
    public interface IPoolService
    {
        T Spawn<T>(
            string poolId,
            T prefab,
            Vector3 position,
            Quaternion rotation)
            where T : Component;

        void Despawn(
            string poolId,
            Component instance);
    }
}
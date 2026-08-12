using UnityEngine;

namespace FireLine.Scripts.Pooling
{
    public interface IPoolService
    {
        T Spawn<T>(
            string poolKey,
            Vector3 position,
            Quaternion rotation)
            where T : Component;

        void Despawn(
            string poolKey,
            Component instance);
    }
}
using System.Collections.Generic;
using UnityEngine;

namespace FireLine.Scripts.Core.Pooling
{
    public class PoolService : IPoolService
    {
        private readonly Dictionary<string, Queue<Component>> _pools = new();

        private readonly Transform _poolRoot;

        public PoolService()
        {
            GameObject root = new GameObject("[PoolService]");
            _poolRoot = root.transform;
        }

        public T Spawn<T>(
            string poolId,
            T prefab,
            Vector3 position,
            Quaternion rotation)
            where T : Component
        {
            if (!_pools.TryGetValue(poolId, out Queue<Component> pool))
            {
                pool = new Queue<Component>();
                _pools.Add(poolId, pool);
            }

            T instance;

            if (pool.Count > 0)
            {
                instance = (T)pool.Dequeue();
            }
            else
            {
                instance = Object.Instantiate(
                    prefab,
                    _poolRoot
                );
            }

            instance.transform.SetPositionAndRotation(
                position,
                rotation
            );

            instance.gameObject.SetActive(true);

            if (instance is IPoolable poolable)
            {
                poolable.OnSpawn();
            }

            return instance;
        }

        public void Despawn(
            string poolId,
            Component instance)
        {
            if (instance == null)
                return;

            if (!_pools.TryGetValue(
                    poolId,
                    out Queue<Component> pool))
            {
                Debug.LogWarning(
                    $"Pool '{poolId}' does not exist."
                );

                return;
            }

            if (instance is IPoolable poolable)
            {
                poolable.OnDespawn();
            }

            instance.gameObject.SetActive(false);

            pool.Enqueue(instance);
        }
    }
}
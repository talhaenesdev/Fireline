using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace FireLine.Scripts.Pooling
{
    public class PoolService : IPoolService
    {
        private class Pool
        {
            public PoolData Data { get; }
            public Queue<GameObject> Objects { get; }

            public Pool(PoolData data)
            {
                Data = data;
                Objects = new Queue<GameObject>();
            }
        }
        private readonly DiContainer _container;
        private readonly Dictionary<string, Pool> _pools = new();

        private readonly Transform _poolRoot;

        public PoolService(
            PoolConfig config,
            DiContainer container)
        {
            _container = container;

            GameObject root = new GameObject("[PoolService]");

            Object.DontDestroyOnLoad(root);

            _poolRoot = root.transform;

            Initialize(config);
        }

        private void Initialize(PoolConfig config)
        {
            if (config == null)
            {
                Debug.LogError("PoolConfig is null.");
                return;
            }

            foreach (PoolData poolData in config.Pools)
            {
                CreatePool(poolData);
            }
        }

        private void CreatePool(PoolData poolData)
        {
            if (poolData == null)
                return;

            if (string.IsNullOrWhiteSpace(poolData.PoolKey))
            {
                Debug.LogError(
                    $"PoolData '{poolData.name}' has empty PoolKey."
                );

                return;
            }

            if (_pools.ContainsKey(poolData.PoolKey))
            {
                Debug.LogError(
                    $"Duplicate PoolKey: {poolData.PoolKey}"
                );

                return;
            }

            if (poolData.Prefab == null)
            {
                Debug.LogError(
                    $"PoolData '{poolData.PoolKey}' has no prefab."
                );

                return;
            }

            Pool pool = new Pool(poolData);

            _pools.Add(
                poolData.PoolKey,
                pool
            );

            for (int i = 0; i < poolData.InitialSize; i++)
            {
                GameObject instance = CreateInstance(pool);

                pool.Objects.Enqueue(instance);
            }
        }

        private GameObject CreateInstance(Pool pool)
        {
            GameObject instance = Object.Instantiate(
                pool.Data.Prefab,
                _poolRoot
            );

            _container.InjectGameObject(instance);

            PooledParticleEffect particleEffect =
                instance.GetComponent<PooledParticleEffect>();

            if (particleEffect != null)
            {
                particleEffect.Initialize(
                    this,
                    pool.Data.PoolKey
                );
            }

            instance.SetActive(false);

            return instance;
        }

        public T Spawn<T>(
            string poolKey,
            Vector3 position,
            Quaternion rotation)
            where T : Component
        {
            if (!_pools.TryGetValue(
                    poolKey,
                    out Pool pool))
            {
                Debug.LogError(
                    $"Pool '{poolKey}' was not found."
                );

                return null;
            }

            GameObject instance = null;

            if (pool.Objects.Count > 0)
            {
                instance = pool.Objects.Dequeue();
            }
            else if (pool.Data.Expandable)
            {
                instance = CreateInstance(pool);
            }

            if (instance == null)
            {
                Debug.LogWarning(
                    $"Pool '{poolKey}' has no available objects."
                );

                return null;
            }

            instance.transform.SetPositionAndRotation(
                position,
                rotation
            );

            instance.SetActive(true);

            T component = instance.GetComponent<T>();

            if (component == null)
            {
                Debug.LogError(
                    $"Pool '{poolKey}' prefab does not contain " +
                    $"{typeof(T).Name}."
                );

                instance.SetActive(false);
                pool.Objects.Enqueue(instance);

                return null;
            }

            if (component is IPoolable poolable)
            {
                poolable.OnSpawn();
            }

            return component;
        }

        public void Despawn(
            string poolKey,
            Component instance)
        {
            if (instance == null)
                return;

            if (!_pools.TryGetValue(
                    poolKey,
                    out Pool pool))
            {
                Debug.LogError(
                    $"Pool '{poolKey}' was not found."
                );

                return;
            }

            if (instance is IPoolable poolable)
            {
                poolable.OnDespawn();
            }

            GameObject gameObject = instance.gameObject;

            gameObject.SetActive(false);

            if (pool.Objects.Count < pool.Data.MaxSize)
            {
                pool.Objects.Enqueue(gameObject);
            }
            else
            {
                Object.Destroy(gameObject);
            }
        }
    }
}
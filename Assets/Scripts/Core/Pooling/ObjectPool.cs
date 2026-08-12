using System.Collections.Generic;
using UnityEngine;

namespace FireLine.Scripts.Core.Pooling
{
    public class ObjectPool<T> where T : Component
    {
        private readonly T _prefab;
        private readonly Transform _parent;
        private readonly Queue<T> _availableObjects = new();

        public ObjectPool(
            T prefab,
            Transform parent,
            int initialSize)
        {
            _prefab = prefab;
            _parent = parent;

            for (int i = 0; i < initialSize; i++)
            {
                T instance = CreateObject();
                _availableObjects.Enqueue(instance);
            }
        }

        private T CreateObject()
        {
            T instance = Object.Instantiate(
                _prefab,
                _parent
            );

            instance.gameObject.SetActive(false);

            return instance;
        }

        public T Get()
        {
            T instance;

            if (_availableObjects.Count > 0)
            {
                instance = _availableObjects.Dequeue();
            }
            else
            {
                instance = CreateObject();
            }

            instance.gameObject.SetActive(true);

            if (instance.TryGetComponent<IPoolable>(out var poolable))
            {
                poolable.OnSpawn();
            }

            return instance;
        }

        public void Release(T instance)
        {
            if (instance == null)
                return;

            if (instance.TryGetComponent<IPoolable>(out var poolable))
            {
                poolable.OnDespawn();
            }

            instance.gameObject.SetActive(false);

            _availableObjects.Enqueue(instance);
        }
    }
}
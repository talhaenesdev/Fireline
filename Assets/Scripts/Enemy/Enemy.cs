using FireLine.Scripts.Core.Services.Entities;
using FireLine.Scripts.Enemy.Data;
using FireLine.Scripts.Pooling;
using UnityEngine;

namespace FireLine.Scripts.Enemy
{
    public class Enemy : Entity, IPoolable, IPoolKeyProvider
    {
        [SerializeField]
        private EnemyData data;

        public string PoolKey => data.PoolKey;

        public void OnSpawn()
        {
            Debug.Log($"Enemy spawned from pool: {name}");

            ResetHealth();
        }

        public void OnDespawn()
        {
            Debug.Log($"Enemy despawned: {name}");
        }
    }
}
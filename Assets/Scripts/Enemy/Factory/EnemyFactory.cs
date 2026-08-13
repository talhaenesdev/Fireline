using FireLine.Scripts.Enemy.Data;
using FireLine.Scripts.Pooling;
using UnityEngine;

namespace FireLine.Scripts.Enemy.Factory
{
    public class EnemyFactory : IEnemyFactory
    {
        private readonly IPoolService _poolService;

        public EnemyFactory(IPoolService poolService)
        {
            _poolService = poolService;
        }

        public Enemy Create(
            EnemyData data,
            Vector3 position,
            Quaternion rotation)
        {
            return _poolService.Spawn<Enemy>(
                data.PoolKey,
                position,
                rotation
            );
        }
    }
}
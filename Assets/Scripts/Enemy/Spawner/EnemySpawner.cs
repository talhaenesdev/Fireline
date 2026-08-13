using FireLine.Scripts.Enemy.Data;
using FireLine.Scripts.Enemy.Factory;
using UnityEngine;

namespace FireLine.Scripts.Enemy.Spawner
{
    public class EnemySpawner
    {
        private readonly IEnemyFactory _enemyFactory;

        public EnemySpawner(IEnemyFactory enemyFactory)
        {
            _enemyFactory = enemyFactory;
        }

        public Enemy Spawn(
            EnemyData data,
            Vector3 position,
            Quaternion rotation)
        {
            return _enemyFactory.Create(
                data,
                position,
                rotation
            );
        }
    }
}
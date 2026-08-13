using FireLine.Scripts.Enemy.Data;
using FireLine.Scripts.Enemy.Factory;
using UnityEngine;
using Zenject;

namespace FireLine.Scripts.Enemy.SpawnPoint
{
    public class EnemySpawnPoint : MonoBehaviour
    {
        private IEnemyFactory _enemyFactory;

        [Inject]
        public void Construct(IEnemyFactory enemyFactory)
        {
            _enemyFactory = enemyFactory;
        }

        public Enemy Spawn(EnemyData data)
        {
            return _enemyFactory.Create(
                data,
                transform.position,
                transform.rotation
            );
        }
    }
}
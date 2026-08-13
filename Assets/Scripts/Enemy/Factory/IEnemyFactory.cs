using FireLine.Scripts.Enemy.Data;
using UnityEngine;

namespace FireLine.Scripts.Enemy.Factory
{
    public interface IEnemyFactory
    {
        Enemy Create(
            EnemyData data,
            Vector3 position,
            Quaternion rotation);
    }
}
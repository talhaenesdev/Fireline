using System;
using UnityEngine;

namespace FireLine.Scripts.Enemy.Data
{
    [Serializable]
    public class EnemySpawnEntry
    {
        public EnemyData enemyData;
        public int count;
    }

    [CreateAssetMenu(
        fileName = "EnemyWaveData",
        menuName = "FireLine/Enemy/Enemy Wave Data"
    )]
    public class EnemyWaveData : ScriptableObject
    {
        [SerializeField]
        private EnemySpawnEntry[] enemies;

        public EnemySpawnEntry[] Enemies => enemies;
    }
}
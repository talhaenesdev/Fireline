using FireLine.Scripts.Enemy.Data;
using FireLine.Scripts.Enemy.SpawnPoint;
using UnityEngine;

namespace FireLine.Scripts.Enemy.Spawner
{
    public class EnemyWaveController : MonoBehaviour
    {
        [SerializeField]
        private EnemyWaveData waveData;

        [SerializeField]
        private EnemySpawnPoint[] spawnPoints;

        public void SpawnWave()
        {
            if (waveData == null)
            {
                Debug.LogError("EnemyWaveData is missing.");
                return;
            }

            if (spawnPoints == null || spawnPoints.Length == 0)
            {
                Debug.LogError("No EnemySpawnPoints assigned.");
                return;
            }

            foreach (var entry in waveData.Enemies)
            {
                for (int i = 0; i < entry.count; i++)
                {
                    var spawnPoint = spawnPoints[i % spawnPoints.Length];

                    spawnPoint.Spawn(entry.enemyData);
                }
            }
        }

        [ContextMenu("Spawn Wave")]
        private void TestSpawnWave()
        {
            SpawnWave();
        }
    }
}
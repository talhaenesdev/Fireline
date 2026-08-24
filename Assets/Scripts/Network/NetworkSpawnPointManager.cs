using UnityEngine;

namespace FireLine.Scripts.Network
{
    public class NetworkSpawnPointManager : MonoBehaviour
    {
        [SerializeField]
        private Transform[] spawnPoints;

        private int _nextSpawnIndex;

        public Transform GetNextSpawnPoint()
        {
            if (spawnPoints == null ||
                spawnPoints.Length == 0)
            {
                Debug.LogError(
                    "No Network Spawn Points configured!"
                );

                return null;
            }

            Transform spawnPoint =
                spawnPoints[_nextSpawnIndex];

            _nextSpawnIndex =
                (_nextSpawnIndex + 1) %
                spawnPoints.Length;

            return spawnPoint;
        }
    }
}
using UnityEngine;

namespace FireLine.Scripts.Network.Service
{
    public class NetworkSpawnPointManager : MonoBehaviour
    {
        [SerializeField]
        private Transform[] spawnPoints;

        public int Count =>
            spawnPoints != null
                ? spawnPoints.Length
                : 0;

        public Transform GetSpawnPoint(
            ulong clientId)
        {
            if (spawnPoints == null ||
                spawnPoints.Length == 0)
            {
                Debug.LogError(
                    "[SPAWN POINT MANAGER] " +
                    "No spawn points assigned!"
                );

                return null;
            }

            int index =
                (int)(
                    clientId %
                    (ulong)spawnPoints.Length
                );

            return spawnPoints[index];
        }
    }
}
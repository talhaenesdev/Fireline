using UnityEngine;

namespace FireLine.Scripts.Network.View
{
    public class NetworkRespawnPoint : MonoBehaviour
    {
        [SerializeField]
        private int spawnIndex;

        public int SpawnIndex =>
            spawnIndex;

        public Vector3 Position =>
            transform.position;

        public Quaternion Rotation =>
            transform.rotation;
    }
}
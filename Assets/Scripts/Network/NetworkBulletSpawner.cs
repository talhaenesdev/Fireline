using Unity.Netcode;
using UnityEngine;

namespace FireLine.Scripts.Network
{
    public class NetworkBulletSpawner : MonoBehaviour
    {
        [SerializeField]
        private NetworkBullet bulletPrefab;

        public void Spawn(
            Vector3 position,
            Vector3 direction)
        {
            Debug.Log(
                "=== BULLET SPAWNER CALLED ==="
            );

            if (!NetworkManager.Singleton.IsServer)
            {
                Debug.LogWarning(
                    "BulletSpawner called on non-server."
                );

                return;
            }

            if (bulletPrefab == null)
            {
                Debug.LogError(
                    "NetworkBullet Prefab NULL!"
                );

                return;
            }

            NetworkBullet bullet =
                Instantiate(
                    bulletPrefab,
                    position,
                    Quaternion.LookRotation(direction)
                );

            NetworkObject networkObject =
                bullet.GetComponent<NetworkObject>();

            if (networkObject == null)
            {
                Debug.LogError(
                    "Bullet prefabında NetworkObject yok!"
                );

                Destroy(bullet.gameObject);
                return;
            }

            networkObject.Spawn();

            bullet.Initialize(direction);

            Debug.Log(
                $"Network Bullet Spawned | " +
                $"NetworkObjectId: " +
                $"{networkObject.NetworkObjectId}"
            );
        }
    }
}
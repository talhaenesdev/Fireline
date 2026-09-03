using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace FireLine.Scripts.Network
{
    public class NetworkBulletSpawner : MonoBehaviour
    {
        [SerializeField]
        private NetworkBullet bulletPrefab;

        public void Spawn(
            Vector3 position,
            Vector3 direction,
            ulong ownerClientId)
        {
            if (NetworkManager.Singleton == null)
            {
                Debug.LogError(
                    "[BULLET SPAWNER] " +
                    "NetworkManager.Singleton is NULL!"
                );

                return;
            }

            if (!NetworkManager.Singleton.IsServer)
            {
                Debug.LogWarning(
                    "[BULLET SPAWNER] " +
                    "Spawn called on non-server!"
                );

                return;
            }

            if (bulletPrefab == null)
            {
                Debug.LogError(
                    "[BULLET SPAWNER] " +
                    "NetworkBullet Prefab is NULL!"
                );

                return;
            }

            if (direction == Vector3.zero)
            {
                Debug.LogWarning(
                    "[BULLET SPAWNER] " +
                    "Direction is zero!"
                );

                return;
            }

            direction.Normalize();

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
                    "[BULLET SPAWNER] " +
                    "NetworkBullet prefabında " +
                    "NetworkObject yok!"
                );

                Destroy(bullet.gameObject);

                return;
            }

            networkObject.Spawn();

            bullet.Initialize(
                direction,
                ownerClientId
            );

            Debug.Log(
                $"[BULLET SPAWNER] " +
                $"Network Bullet Spawned | " +
                $"NetworkObjectId: {networkObject.NetworkObjectId} | " +
                $"OwnerClientId: {ownerClientId} | " +
                $"Position: {position} | " +
                $"Direction: {direction}"
            );
        }
    }
}
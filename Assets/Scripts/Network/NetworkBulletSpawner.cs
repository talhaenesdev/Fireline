using Unity.Netcode;
using UnityEngine;

namespace FireLine.Scripts.Network
{
    public class NetworkBulletSpawner : MonoBehaviour
    {
        [Header("Network Bullet")]
        [SerializeField]
        private NetworkBullet bulletPrefab;

        [Header("Predicted Bullet")]
        [SerializeField]
        private PredictedBullet predictedBulletPrefab;

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

            networkObject.SpawnWithOwnership(ownerClientId);

            bullet.Initialize(
                direction,
                ownerClientId
            );

            Debug.Log(
                $"[BULLET SPAWNER] " +
                $"Network Bullet Spawned | " +
                $"NetworkObjectId: " +
                $"{networkObject.NetworkObjectId} | " +
                $"OwnerClientId: {ownerClientId}"
            );
        }

        public void SpawnPredicted(
    Vector3 position,
    Vector3 direction,
    float speed,
    float lifetime,
    Transform ownerTransform)
        {




            if (predictedBulletPrefab == null)
            {
                Debug.LogError(
                    "[BULLET SPAWNER] " +
                    "PredictedBullet Prefab is NULL!"
                );

                return;
            }

            if (direction == Vector3.zero)
                return;

            PredictedBullet bullet =
                Instantiate(
                    predictedBulletPrefab,
                    position,
                    Quaternion.LookRotation(direction)
                );


            Debug.Log(
    $"[PREDICTED BULLET] Spawn | " +
    $"Position={position} | " +
    $"Direction={direction} | " +
    $"Speed={speed} | " +
    $"Lifetime={lifetime}"
);

            bullet.Initialize(
                direction,
                speed,
                lifetime,
                ownerTransform
            );
        }
    }
}
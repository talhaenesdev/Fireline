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
            Vector3 direction,
            ulong ownerClientId)
        {
            if (!NetworkManager.Singleton.IsServer)
                return;

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

            bullet.Initialize(
                direction,
                ownerClientId
            );

            Debug.Log(
                $"Network Bullet Spawned | " +
                $"OwnerClientId: {ownerClientId}"
            );
        }
    }
}
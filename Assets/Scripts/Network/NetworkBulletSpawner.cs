using FireLine.Scripts.Network.Signals;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace FireLine.Scripts.Network
{
    public class NetworkBulletSpawner : MonoBehaviour
    {
        [SerializeField]
        private NetworkBullet bulletPrefab;

        private SignalBus _signalBus;

        [Inject]
        public void Construct(
            SignalBus signalBus)
        {
            _signalBus = signalBus;

            _signalBus.Subscribe<
                NetworkShootSignal>(
                OnShootRequested
            );
        }

        private void OnShootRequested(
            NetworkShootSignal signal)
        {
            Spawn(
                signal.Position,
                signal.Direction,
                signal.ClientId
            );
        }

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
                    "Bullet prefabında " +
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
                $"Network Bullet Spawned | " +
                $"OwnerClientId: {ownerClientId}"
            );
        }

        private void OnDestroy()
        {
            if (_signalBus != null)
            {
                _signalBus.TryUnsubscribe<
                    NetworkShootSignal>(
                    OnShootRequested
                );
            }
        }
    }
}
using FireLine.Scripts.Core.Weapon;
using Unity.Netcode;
using UnityEngine;

namespace FireLine.Scripts.Network
{
    public class NetworkWeaponFireService :
        NetworkBehaviour,
        IWeaponFireService
    {
        private NetworkBulletSpawner _bulletSpawner;

        private void Awake()
        {
            _bulletSpawner =
                FindFirstObjectByType<
                    NetworkBulletSpawner>();

            if (_bulletSpawner == null)
            {
                Debug.LogError(
                    "[NETWORK WEAPON] " +
                    "NetworkBulletSpawner NOT FOUND!"
                );
            }
        }

        public void Fire(
            Vector3 position,
            Vector3 direction)
        {
            if (!IsOwner)
                return;

            if (direction == Vector3.zero)
                return;

            RequestFireServerRpc(
                position,
                direction
            );
        }

        [ServerRpc]
        private void RequestFireServerRpc(
            Vector3 position,
            Vector3 direction,
            ServerRpcParams rpcParams = default)
        {
            if (!IsServer)
                return;

            if (_bulletSpawner == null)
            {
                Debug.LogError(
                    "[NETWORK WEAPON] " +
                    "NetworkBulletSpawner is NULL!"
                );

                return;
            }

            ulong clientId =
                rpcParams.Receive.SenderClientId;

            _bulletSpawner.Spawn(
                position,
                direction,
                clientId
            );

            Debug.Log(
                $"[NETWORK WEAPON] " +
                $"Server spawned bullet | " +
                $"ClientId: {clientId}"
            );
        }
    }
}
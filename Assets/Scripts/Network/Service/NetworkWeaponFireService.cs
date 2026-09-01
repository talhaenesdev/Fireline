using FireLine.Scripts.Core.Weapon;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace FireLine.Scripts.Network
{
    public class NetworkWeaponFireService :
        NetworkBehaviour,
        IWeaponFireService
    {
        private NetworkBulletSpawner _bulletSpawner;

        [Inject]
        public void Construct(
            NetworkBulletSpawner bulletSpawner)
        {
            _bulletSpawner = bulletSpawner;

            Debug.Log(
                $"[NETWORK WEAPON] Construct | " +
                $"BulletSpawner: {_bulletSpawner != null}"
            );
        }

        public void Fire(
            Vector3 position,
            Vector3 direction)
        {
            if (!IsOwner)
            {
                Debug.Log(
                    "[NETWORK WEAPON] " +
                    "Fire rejected: not owner."
                );

                return;
            }

            if (direction == Vector3.zero)
                return;

            Debug.Log(
                $"[NETWORK WEAPON] Fire | " +
                $"Position: {position} | " +
                $"Direction: {direction}"
            );

            RequestFireServerRpc(
                position,
                direction.normalized
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

            if (direction == Vector3.zero)
                return;

            ulong clientId =
                rpcParams.Receive.SenderClientId;

            Debug.Log(
                $"[NETWORK WEAPON] " +
                $"Server received fire request | " +
                $"ClientId: {clientId} | " +
                $"Position: {position} | " +
                $"Direction: {direction}"
            );

            _bulletSpawner.Spawn(
                position,
                direction.normalized,
                clientId
            );
        }
    }
}
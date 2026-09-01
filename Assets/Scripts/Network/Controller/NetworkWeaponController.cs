using FireLine.Scripts.Weapon.Controller;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace FireLine.Scripts.Network.Controller
{
    public class NetworkWeaponController :
        NetworkBehaviour
    {
        private WeaponController _weaponController;
        private NetworkBulletSpawner _bulletSpawner;

        [Inject]
        public void Construct(
            WeaponController weaponController,
            NetworkBulletSpawner bulletSpawner)
        {
            _weaponController =
                weaponController;

            _bulletSpawner =
                bulletSpawner;

            Debug.Log(
                "[NETWORK WEAPON] " +
                "Dependencies injected."
            );
        }

        [ServerRpc]
        public void ShootServerRpc(
            Vector3 position,
            Vector3 direction,
            ServerRpcParams rpcParams = default)
        {
            if (!IsServer)
                return;

            if (_weaponController == null)
            {
                Debug.LogError(
                    "[NETWORK WEAPON] " +
                    "WeaponController is NULL!"
                );

                return;
            }

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

            if (!_weaponController.CanShoot())
                return;

            ulong clientId =
                rpcParams.Receive.SenderClientId;

            _bulletSpawner.Spawn(
                position,
                direction,
                clientId
            );

            _weaponController.RegisterShot();

            Debug.Log(
                $"[SERVER] Weapon fired | " +
                $"ClientId: {clientId}"
            );
        }
    }
}
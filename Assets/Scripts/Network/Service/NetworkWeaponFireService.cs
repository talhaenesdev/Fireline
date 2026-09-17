using FireLine.Scripts.Core.Weapon;
using FireLine.Scripts.Weapon.Controller;
using FireLine.Scripts.Weapon.Model;
using FireLine.Scripts.Weapon.Service;
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
        private WeaponController _weaponController;

        [Inject]
        public void Construct(
            NetworkBulletSpawner bulletSpawner,
            WeaponController weaponController)
        {
            _bulletSpawner =
                bulletSpawner;

            _weaponController =
                weaponController;

            Debug.Log(
                $"[NETWORK WEAPON] Construct | " +
                $"BulletSpawner={_bulletSpawner != null} | " +
                $"WeaponController={_weaponController != null}"
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

            direction.Normalize();

            Debug.Log(
                $"[NETWORK WEAPON] Fire | " +
                $"Position={position} | " +
                $"Direction={direction}"
            );

            PlayLocalFireSound(
                position
            );

            SpawnPredictedBullet(
                position,
                direction
            );

            RequestFireServerRpc(
                position,
                direction
            );
        }

        private void SpawnPredictedBullet(
            Vector3 position,
            Vector3 direction)
        {
            if (_bulletSpawner == null)
            {
                Debug.LogError(
                    "[NETWORK WEAPON] " +
                    "BulletSpawner is NULL!"
                );

                return;
            }

            if (_weaponController == null)
            {
                Debug.LogError(
                    "[NETWORK WEAPON] " +
                    "WeaponController is NULL!"
                );

                return;
            }

            BulletData bulletData =
                _weaponController.BulletData;

            if (bulletData == null)
            {
                Debug.LogError(
                    "[NETWORK WEAPON] " +
                    "BulletData is NULL!"
                );

                return;
            }

            _bulletSpawner.SpawnPredicted(
                position,
                direction,
                bulletData.Speed,
                bulletData.Lifetime,
                transform
            );
        }

        private void PlayLocalFireSound(
            Vector3 position)
        {
            if (WeaponAudioManager.Instance == null)
            {
                Debug.LogWarning(
                    "[NETWORK WEAPON] " +
                    "WeaponAudioManager is NULL!"
                );

                return;
            }

            WeaponAudioManager.Instance.PlayFireSound(
                position
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
                $"[NETWORK WEAPON] SERVER FIRE | " +
                $"Client={clientId} | " +
                $"Position={position} | " +
                $"Direction={direction} | " +
                $"PlayerPosition={transform.position}"
            );

            _bulletSpawner.Spawn(
                position,
                direction,
                clientId
            );

            PlayWeaponFireClientRpc(
                position,
                GetOtherClientIds(clientId)
            );
        }

        private ClientRpcParams GetOtherClientIds(
            ulong excludedClientId)
        {
            if (NetworkManager == null)
                return default;

            var connectedClients =
                NetworkManager.ConnectedClientsIds;

            ulong[] targetClientIds =
                new ulong[
                    connectedClients.Count
                ];

            int index = 0;

            foreach (ulong clientId
                     in connectedClients)
            {
                if (clientId == excludedClientId)
                    continue;

                targetClientIds[index] =
                    clientId;

                index++;
            }

            if (index != targetClientIds.Length)
            {
                ulong[] resizedTargets =
                    new ulong[index];

                for (int i = 0;
                     i < index;
                     i++)
                {
                    resizedTargets[i] =
                        targetClientIds[i];
                }

                targetClientIds =
                    resizedTargets;
            }

            return new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds =
                        targetClientIds
                }
            };
        }

        [ClientRpc]
        private void PlayWeaponFireClientRpc(
            Vector3 position,
            ClientRpcParams clientRpcParams = default)
        {
            if (WeaponAudioManager.Instance == null)
            {
                Debug.LogWarning(
                    "[NETWORK WEAPON] " +
                    "WeaponAudioManager is NULL!"
                );

                return;
            }

            WeaponAudioManager.Instance.PlayFireSound(
                position
            );
        }
    }
}
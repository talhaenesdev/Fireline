using FireLine.Scripts.Core.Services.Entities;
using FireLine.Scripts.Player.Controller;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace FireLine.Scripts.Network
{
    public class NetworkPlayer : NetworkBehaviour
    {
        private PlayerInputController _playerInputController;
        private PlayerInput _playerInput;

        private PlayerMovementController _movementController;
        private PlayerAimController _aimController;
        private PlayerGameplayController _gameplayController;

        private NetworkBulletSpawner _bulletSpawner;
        private Entity _entity;
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            SceneContext sceneContext =
                FindFirstObjectByType<SceneContext>();

            if (sceneContext != null)
            {
                sceneContext.Container.InjectGameObject(gameObject);

                Debug.Log(
                    "Zenject injection completed for NetworkPlayer."
                );
            }
            else
            {
                Debug.LogError(
                    "SceneContext not found!"
                );
            }

            _playerInputController =
                GetComponent<PlayerInputController>();

            _playerInput =
                GetComponent<PlayerInput>();

            _movementController =
                GetComponent<PlayerMovementController>();

            _aimController =
                GetComponent<PlayerAimController>();

            _gameplayController =
                GetComponent<PlayerGameplayController>();

            _bulletSpawner =
                FindFirstObjectByType<NetworkBulletSpawner>();

            Debug.Log(
                $"NetworkBulletSpawner found: " +
                $"{_bulletSpawner != null}"
            );

            if (_playerInputController != null)
                _playerInputController.enabled = IsOwner;

            if (_playerInput != null)
                _playerInput.enabled = IsOwner;

            if (_movementController != null)
                _movementController.enabled = IsOwner;

            if (_aimController != null)
                _aimController.enabled = IsOwner;

            if (_gameplayController != null)
            {
                _gameplayController.enabled = IsOwner;

                if (IsOwner)
                {
                    _gameplayController.OnFire +=
                        HandleFire;
                }
            }

            Debug.Log(
                $"Network Player Spawned | " +
                $"ClientId: {OwnerClientId} | " +
                $"IsOwner: {IsOwner} | " +
                $"IsServer: {IsServer}"
            );
        }

        private void HandleFire(Vector3 direction)
        {
            if (!IsOwner)
                return;

            Vector3 muzzlePosition =
                _gameplayController.MuzzlePosition;

            Debug.Log(
                $"CLIENT FIRE | " +
                $"Muzzle: {muzzlePosition} | " +
                $"Direction: {direction}"
            );

            ShootServerRpc(
                muzzlePosition,
                direction
            );
        }

        [ServerRpc]
        private void ShootServerRpc(
            Vector3 position,
            Vector3 direction)
        {
            Debug.Log(
                $"SERVER SHOOT RPC | " +
                $"Position: {position} | " +
                $"Direction: {direction}"
            );

            if (!IsServer)
                return;

            if (_bulletSpawner == null)
            {
                Debug.LogError(
                    "NetworkBulletSpawner is NULL on SERVER!"
                );

                return;
            }

            _bulletSpawner.Spawn(
                position,
                direction,
                OwnerClientId
            );
        }

        public override void OnNetworkDespawn()
        {

            _entity =
                GetComponent<Entity>();

            if (_gameplayController != null &&
                IsOwner)
            {
                _gameplayController.OnFire -=
                    HandleFire;
            }

            base.OnNetworkDespawn();
        }
    }
}
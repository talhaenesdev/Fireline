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

        private NetworkPlayerHealth _health;

        private bool _healthSubscribed;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            Debug.Log(
                $"[NETWORK PLAYER] Spawned | " +
                $"ClientId: {OwnerClientId} | " +
                $"IsOwner: {IsOwner} | " +
                $"IsServer: {IsServer} | " +
                $"IsClient: {IsClient}"
            );

            InjectWithSceneContext();

            FindComponents();

            SetupOwner();

            SubscribeHealth();

            Debug.Log(
                "[NETWORK PLAYER] Setup completed."
            );
        }

        private void InjectWithSceneContext()
        {
            SceneContext sceneContext =
                FindFirstObjectByType<SceneContext>();

            if (sceneContext == null)
            {
                Debug.LogError(
                    "[NETWORK PLAYER] " +
                    "SceneContext NOT FOUND!"
                );

                return;
            }

            sceneContext.Container
                .InjectGameObject(gameObject);

            Debug.Log(
                "[NETWORK PLAYER] " +
                "Zenject injection completed."
            );
        }

        private void FindComponents()
        {
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

            _health =
                GetComponent<NetworkPlayerHealth>();
        }

        private void SetupOwner()
        {
            if (!IsOwner)
            {
                DisableLocalInput();
                return;
            }

            EnableLocalInput();

            Debug.Log(
                $"[NETWORK PLAYER] " +
                $"Local owner initialized | " +
                $"ClientId: {OwnerClientId}"
            );
        }

        private void SubscribeHealth()
        {
            if (_health == null)
            {
                Debug.LogError(
                    "[NETWORK PLAYER] " +
                    "NetworkPlayerHealth NOT FOUND!"
                );

                return;
            }

            if (_healthSubscribed)
                return;

            _health.DeathStateChanged +=
                OnDeathStateChanged;

            _healthSubscribed = true;
        }

        private void OnDeathStateChanged(
            bool isDead)
        {
            if (!IsOwner)
                return;

            Debug.Log(
                $"[NETWORK PLAYER] " +
                $"Death state changed | " +
                $"ClientId: {OwnerClientId} | " +
                $"IsDead: {isDead}"
            );

            if (isDead)
            {
                DisableLocalInput();
            }
            else
            {
                EnableLocalInput();
            }
        }

        private void EnableLocalInput()
        {
            if (_playerInput != null)
                _playerInput.enabled = true;

            if (_playerInputController != null)
                _playerInputController.enabled = true;

            if (_movementController != null)
                _movementController.enabled = true;

            if (_aimController != null)
                _aimController.enabled = true;

            if (_gameplayController != null)
                _gameplayController.enabled = true;
        }

        private void DisableLocalInput()
        {
            if (_playerInput != null)
                _playerInput.enabled = false;

            if (_playerInputController != null)
                _playerInputController.enabled = false;

            if (_movementController != null)
                _movementController.enabled = false;

            if (_aimController != null)
                _aimController.enabled = false;

            if (_gameplayController != null)
                _gameplayController.enabled = false;
        }

        public override void OnNetworkDespawn()
        {
            if (_health != null &&
                _healthSubscribed)
            {
                _health.DeathStateChanged -=
                    OnDeathStateChanged;

                _healthSubscribed = false;
            }

            DisableLocalInput();

            Debug.Log(
                $"[NETWORK PLAYER] Despawned | " +
                $"ClientId: {OwnerClientId}"
            );

            base.OnNetworkDespawn();
        }
    }
}
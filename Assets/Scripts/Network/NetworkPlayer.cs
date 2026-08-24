using FireLine.Scripts.Network.Signals;
using FireLine.Scripts.Player.Controller;
using FireLine.Scripts.Weapon.Controller;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace FireLine.Scripts.Network
{
    public class NetworkPlayer : NetworkBehaviour
    {
        private SignalBus _signalBus;
        private PlayerInputController _playerInputController;
        private PlayerInput _playerInput;
        private WeaponController _weaponController;
        private PlayerMovementController _movementController;
        private PlayerAimController _aimController;
        private PlayerGameplayController _gameplayController;

        private NetworkPlayerHealth _health;
        [Inject]
        public void Construct(
    SignalBus signalBus)
        {
            _signalBus = signalBus;
        }
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            Debug.Log(
                $"[NETWORK PLAYER] Spawned | " +
                $"ClientId: {OwnerClientId} | " +
                $"IsOwner: {IsOwner} | " +
                $"IsServer: {IsServer}"
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

            _health.DeathStateChanged +=
                OnDeathStateChanged;
        }

        private void OnDeathStateChanged(
            bool isDead)
        {
            if (!IsOwner)
                return;

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
            if (_health != null)
            {
                _health.DeathStateChanged -=
                    OnDeathStateChanged;
            }

            base.OnNetworkDespawn();
        }
        [ServerRpc]
        public void RequestShootServerRpc(
    Vector3 position,
    Vector3 direction,
    ServerRpcParams rpcParams = default)
        {
            if (!IsServer)
                return;

            if (_signalBus == null)
            {
                Debug.LogError(
                    "[NETWORK PLAYER] " +
                    "SignalBus is NULL!"
                );

                return;
            }

            ulong clientId =
                rpcParams.Receive.SenderClientId;

            Debug.Log(
                $"[SERVER] Shoot request | " +
                $"ClientId: {clientId}"
            );

            _signalBus.Fire(
                new NetworkShootSignal(
                    position,
                    direction,
                    clientId
                )
            );
        }
    }
}
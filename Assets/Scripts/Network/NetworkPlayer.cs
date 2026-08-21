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

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            SceneContext sceneContext = FindFirstObjectByType<SceneContext>();

            if (sceneContext != null)
            {
                sceneContext.Container.InjectGameObject(gameObject);
                Debug.Log("Zenject injection completed for NetworkPlayer.");
            }
            else
            {
                Debug.LogError("SceneContext not found!");
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

            bool isOwner = IsOwner;

            if (_playerInputController != null)
                _playerInputController.enabled = isOwner;

            if (_playerInput != null)
                _playerInput.enabled = isOwner;

            if (_movementController != null)
                _movementController.enabled = isOwner;

            if (_aimController != null)
                _aimController.enabled = isOwner;

            if (_gameplayController != null)
                _gameplayController.enabled = isOwner;

            Debug.Log(
                $"Network Player Spawned | " +
                $"ClientId: {OwnerClientId} | " +
                $"IsOwner: {IsOwner} | " +
                $"IsServer: {IsServer}"
            );
        }
    }
}
using FireLine.Scripts.Network.Service;
using FireLine.Scripts.Player.Controller;
using FireLine.Scripts.Player.Service;
using System;
using System.Collections;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Zenject;

namespace FireLine.Scripts.Network
{
    public class NetworkPlayer : NetworkBehaviour
    {
        private PlayerInputController _playerInputController;
        private PlayerInput _playerInput;
        private PlayerNameService _playerNameService;
        private NetworkPlayerScoreService _scoreService;
        private PlayerMovementController _movementController;
        private PlayerAimController _aimController;
        private PlayerGameplayController _gameplayController;
        private NetworkPlayerHealth _health;

        private bool _healthSubscribed;
        private bool _injected;

        private readonly NetworkVariable<FixedString64Bytes> _playerName =
                new NetworkVariable<FixedString64Bytes>(
                default,
                NetworkVariableReadPermission.Everyone,
                NetworkVariableWritePermission.Server
        );


        [Inject]
        public void Construct(
            PlayerNameService playerNameService,
            NetworkPlayerScoreService scoreService)
        {
            _playerNameService = playerNameService;
            _scoreService = scoreService;

            Debug.Log(
                $"[NET-PLAYER][NAME] PlayerNameService injected | Player={gameObject.name}"
            );
        }
        public string PlayerName =>
            _playerName.Value.ToString();
        // ============================================================
        // NETWORK SPAWN
        // ============================================================

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            Debug.Log(
                $"[NET-PLAYER][SPAWN] " +
                $"Player={gameObject.name} | " +
                $"Scene={gameObject.scene.name} | " +
                $"ClientId={OwnerClientId} | " +
                $"NetworkObjectId={NetworkObjectId} | " +
                $"Owner={IsOwner} | " +
                $"Server={IsServer} | " +
                $"Client={IsClient}"
            );

            FindComponents();

            Debug.Log(
                $"[NET-PLAYER][COMPONENTS] " +
                $"Input={_playerInput != null} | " +
                $"InputController={_playerInputController != null} | " +
                $"Movement={_movementController != null} | " +
                $"Aim={_aimController != null} | " +
                $"Gameplay={_gameplayController != null} | " +
                $"Health={_health != null}"
            );

            if (SceneManager.GetActiveScene().name == "Game")
            {
                StartCoroutine(
                    WaitForSceneContextAndInject()
                );
            }
            else
            {
                DisableLocalInput();

                Debug.Log(
                    $"[NET-PLAYER][INPUT] " +
                    $"Gameplay disabled outside Game | " +
                    $"Scene={SceneManager.GetActiveScene().name} | " +
                    $"ClientId={OwnerClientId}"
                );
            }

            SubscribeHealth();
        }
        private void SendPlayerName()
        {
            if (_playerNameService == null)
            {
                Debug.LogError(
                    "[NET-PLAYER][NAME] " +
                    "PlayerNameService is NULL!"
                );

                return;
            }

            string playerName =
                _playerNameService.GetPlayerName();

            if (string.IsNullOrWhiteSpace(playerName))
            {
                playerName = "Player";
            }

            playerName = playerName.Trim();

            Debug.Log(
                $"[NET-PLAYER][NAME] " +
                $"Sending name to server | " +
                $"Name={playerName} | " +
                $"ClientId={OwnerClientId}"
            );

            SetPlayerNameServerRpc(
                playerName
            );
        }

        [ServerRpc]
        private void SetPlayerNameServerRpc(string playerName)
        {
            if (!IsServer)
                return;

            if (string.IsNullOrWhiteSpace(playerName))
                playerName = "Player";

            playerName = playerName.Trim();

            if (playerName.Length > 32)
                playerName = playerName.Substring(0, 32);

            _playerName.Value =
                new FixedString64Bytes(playerName);

            Debug.Log(
                $"[NET-PLAYER][NAME] Name registered | " +
                $"ClientId={OwnerClientId} | " +
                $"Name={playerName}"
            );

            if (_scoreService == null)
            {
                Debug.LogError(
                    "[NET-PLAYER][SCORE] " +
                    "NetworkPlayerScoreService is NULL!"
                );

                return;
            }

            _scoreService.UpdatePlayerName(
                OwnerClientId,
                playerName
            );
        }


        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(
            Scene scene,
            LoadSceneMode mode)
        {
            Debug.Log(
                $"[NET-PLAYER][SCENE] Scene loaded | " +
                $"Scene={scene.name}"
            );

            if (scene.name != "Game")
                return;

            if (_injected)
                return;

            StartCoroutine(
                WaitForSceneContextAndInject()
            );
        }

        // ============================================================
        // ZENJECT INJECTION
        // ============================================================

        private IEnumerator WaitForSceneContextAndInject()
        {
            if (_injected)
            {
                Debug.Log(
                    $"[NET-PLAYER][INJECT] Already injected | " +
                    $"Player={gameObject.name}"
                );

                yield break;
            }

            Debug.Log(
                $"[NET-PLAYER][SCENE] Searching SceneContext | " +
                $"PlayerScene={gameObject.scene.name}"
            );

            SceneContext context = null;

            while (context == null)
            {
                context = FindSceneContext();

                if (context == null)
                {
                    Debug.Log(
                        $"[NET-PLAYER][SCENE] SceneContext not found yet | " +
                        $"PlayerScene={gameObject.scene.name}"
                    );

                    yield return null;
                }
            }

            Debug.Log(
                $"[NET-PLAYER][SCENE] SceneContext found | " +
                $"Scene={context.gameObject.scene.name} | " +
                $"Instance={context.GetInstanceID()}"
            );

            InjectWithSceneContext(context);
        }

        private void InjectWithSceneContext(SceneContext context)
        {
            Debug.Log(
                $"[NET-PLAYER][INJECT] " +
                $"Context={context.GetInstanceID()} | " +
                $"Container={context.Container.GetHashCode()}"
            );
            if (_injected)
            {
                Debug.Log(
                    $"[NET-PLAYER][INJECT] Skipped | Already injected | " +
                    $"Player={gameObject.name}"
                );

                return;
            }

            if (context == null)
            {
                Debug.LogError(
                    $"[NET-PLAYER][INJECT][ERROR] " +
                    $"SceneContext is NULL | " +
                    $"PlayerScene={gameObject.scene.name}"
                );

                return;
            }

            Debug.Log(
                $"[NET-PLAYER][INJECT] Starting | " +
                $"Player={gameObject.name} | " +
                $"PlayerScene={gameObject.scene.name} | " +
                $"ContextScene={context.gameObject.scene.name} | " +
                $"Context={context.GetInstanceID()}"
            );
            try
            {
                SignalBus signalBus =
                    context.Container.Resolve<SignalBus>();

                Debug.Log(
                    $"[NET-PLAYER][SIGNAL] SignalBus resolved | " +
                    $"Context={context.GetInstanceID()} | " +
                    $"SignalBus={signalBus.GetHashCode()}"
                );
            }
            catch (Exception exception)
            {
                Debug.LogError(
                    $"[NET-PLAYER][SIGNAL] SignalBus resolve FAILED | " +
                    $"Context={context.GetInstanceID()} | " +
                    $"Scene={context.gameObject.scene.name} | " +
                    $"Reason={exception.Message}"
                );
            }
            try
            {
                context.Container.InjectGameObject(
                    gameObject
                );

                _injected = true;

                Debug.Log(
                    $"[NET-PLAYER][INJECT][SUCCESS] " +
                    $"Zenject injection completed | " +
                    $"Player={gameObject.name} | " +
                    $"Scene={gameObject.scene.name}"
                );
            }
            catch (Exception exception)
            {
                Debug.LogError(
                    $"[NET-PLAYER][INJECT][FAILED] " +
                    $"Player={gameObject.name} | " +
                    $"Scene={gameObject.scene.name}\n" +
                    $"Reason: {exception.Message}"
                );

                return;
            }

            FindComponents();

            SetupOwner();

            if (IsOwner)
            {
                SendPlayerName();
            }
        }

        // ============================================================
        // COMPONENTS
        // ============================================================

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

        // ============================================================
        // OWNER / INPUT
        // ============================================================

        private void SetupOwner()
        {
            if (!IsOwner)
            {
                Debug.Log(
                    $"[NET-PLAYER][INPUT] Remote player | " +
                    $"ClientId={OwnerClientId} | Input disabled"
                );

                DisableLocalInput();

                return;
            }

            Debug.Log(
                $"[NET-PLAYER][INPUT] Local owner | " +
                $"ClientId={OwnerClientId} | Input enabled"
            );

            EnableLocalInput();
        }

        private void EnableLocalInput()
        {
            if (!IsOwner)
                return;

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

            Debug.Log(
                $"[NET-PLAYER][INPUT] ENABLED | " +
                $"ClientId={OwnerClientId}"
            );
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

            Debug.Log(
                $"[NET-PLAYER][INPUT] DISABLED | " +
                $"ClientId={OwnerClientId}"
            );
        }

        // ============================================================
        // HEALTH
        // ============================================================

        private void SubscribeHealth()
        {
            if (_health == null)
            {
                Debug.LogError(
                    $"[NET-PLAYER][HEALTH][ERROR] " +
                    $"NetworkPlayerHealth not found | " +
                    $"Player={gameObject.name}"
                );

                return;
            }

            if (_healthSubscribed)
                return;

            _health.DeathStateChanged +=
                OnDeathStateChanged;

            _healthSubscribed = true;

            Debug.Log(
                $"[NET-PLAYER][HEALTH] " +
                $"DeathStateChanged subscribed | " +
                $"ClientId={OwnerClientId}"
            );
        }

        private void OnDeathStateChanged(bool isDead)
        {
            if (!IsOwner)
                return;

            Debug.Log(
                $"[NET-PLAYER][HEALTH] " +
                $"DeathStateChanged | " +
                $"ClientId={OwnerClientId} | " +
                $"Dead={isDead}"
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

        // ============================================================
        // SCENE CONTEXT
        // ============================================================

        private SceneContext FindSceneContext()
        {
            Scene activeScene =
                SceneManager.GetActiveScene();

            SceneContext[] contexts =
                FindObjectsByType<SceneContext>(
                    FindObjectsInactive.Include,
                    FindObjectsSortMode.None
                );

            foreach (SceneContext context in contexts)
            {
                if (context == null)
                    continue;

                if (context.gameObject.scene != activeScene)
                    continue;

                Debug.Log(
                    $"[NET-PLAYER][SCENE] MATCH | " +
                    $"Scene={activeScene.name} | " +
                    $"Context={context.GetInstanceID()}"
                );

                return context;
            }

            return null;
        }

        // ============================================================
        // NETWORK DESPAWN
        // ============================================================

        public override void OnNetworkDespawn()
        {
            if (_health != null &&
                _healthSubscribed)
            {
                _health.DeathStateChanged -=
                    OnDeathStateChanged;

                _healthSubscribed = false;

                Debug.Log(
                    $"[NET-PLAYER][HEALTH] " +
                    $"DeathStateChanged unsubscribed | " +
                    $"ClientId={OwnerClientId}"
                );
            }

            DisableLocalInput();

            Debug.Log(
                $"[NET-PLAYER][DESPAWN] " +
                $"Player={gameObject.name} | " +
                $"ClientId={OwnerClientId} | " +
                $"NetworkObjectId={NetworkObjectId}"
            );

            base.OnNetworkDespawn();
        }
    }
}
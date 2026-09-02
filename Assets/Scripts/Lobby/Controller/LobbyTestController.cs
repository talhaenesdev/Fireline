
using FireLine.Scripts.Network.Service;
using FireLine.Scripts.Player.Service;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace FireLine.Scripts.Lobby.Controller
{
    public class LobbyTestController : MonoBehaviour
    {
        private PlayerNameService _playerNameService;
        private NetworkLobbyService _lobbyService;
        private NetworkGameStartService _gameStartService;
        [SerializeField]
        private GameObject lobbyPanel;

        [Header("Buttons")]
        [SerializeField]
        private Button createButton;

        [SerializeField]
        private Button joinButton;

        [SerializeField]
        private Button leaveButton;

        [SerializeField]
        private Button readyButton;

        [SerializeField]
        private Button startGameButton;

        [Header("Input")]
        [SerializeField]
        private TMP_InputField joinCodeInput;

        [SerializeField]
        private TMP_InputField playerNameInput;

        [Header("Texts")]
        [SerializeField]
        private TMP_Text sessionCodeText;

        [SerializeField]
        private TMP_Text statusText;

        [SerializeField]
        private TMP_Text hostStatusText;

        [SerializeField]
        private TMP_Text playerCountText;

        [SerializeField]
        private TMP_Text readyText;
        private bool _isBusy;

        [Inject]
        public void Construct(
                NetworkLobbyService lobbyService,
                NetworkGameStartService gameStartService,
                PlayerNameService playerNameService)
        {
            _lobbyService = lobbyService;
            _gameStartService = gameStartService;
            _playerNameService = playerNameService;

            Debug.Log(
                "[LOBBY TEST] LobbyService injected."
            );

            Debug.Log(
                "[LOBBY TEST] GameStartService injected."
            );

            Debug.Log(
                "[LOBBY TEST] PlayerNameService injected."
            );
        }

        private void Start()
        {
            if (playerNameInput != null &&
                _playerNameService != null)
            {
                playerNameInput.text =
                    _playerNameService.GetPlayerName();
            }

            startGameButton.onClick.AddListener(
                OnStartGameButtonClicked
            );

            readyButton.onClick.AddListener(
                OnReadyButtonClicked
            );

            _lobbyService.ReadyChanged +=
                OnReadyChanged;

            createButton.onClick.AddListener(
                OnCreateButtonClicked
            );

            joinButton.onClick.AddListener(
                OnJoinButtonClicked
            );

            leaveButton.onClick.AddListener(
                OnLeaveButtonClicked
            );

            RefreshUI();

            SetStatus("Ready to create or join a lobby.");

            _lobbyService.PlayerCountChanged += OnPlayerCountChanged;
        }

        private void OnDestroy()
        {
            if (startGameButton != null)
            {
                startGameButton.onClick.RemoveListener(
                    OnStartGameButtonClicked
                );
            }

            if (createButton != null)
            {
                createButton.onClick.RemoveListener(
                    OnCreateButtonClicked
                );
            }

            if (joinButton != null)
            {
                joinButton.onClick.RemoveListener(OnJoinButtonClicked);
            }

            if (leaveButton != null)
            {
                leaveButton.onClick.RemoveListener(
                    OnLeaveButtonClicked
                );
            }

            if (_lobbyService != null)
            {
                _lobbyService.PlayerCountChanged -= OnPlayerCountChanged;
            }

            if (readyButton != null)
            {
                readyButton.onClick.RemoveListener(OnReadyButtonClicked);
            }

            if (_lobbyService != null)
            {
                _lobbyService.ReadyChanged -= OnReadyChanged;
            }
        }


        private async Task StartGame()
        {
            if (_isBusy)
                return;

            _isBusy = true;

            Debug.Log(
                "[LOBBY TEST] Requesting game start..."
            );

            bool allowed =
                _lobbyService.StartGame();

            if (!allowed)
            {
                Debug.LogWarning(
                    "[LOBBY TEST] Game start rejected."
                );

                _isBusy = false;
                return;
            }

            bool networkStarted =
                await _gameStartService.StartGame(
                    _lobbyService.Session
                );

            if (!networkStarted)
            {
                Debug.LogError(
                    "[LOBBY TEST] Network start failed."
                );

                _isBusy = false;
                return;
            }

            Debug.Log(
                "[LOBBY TEST] Game start completed."
            );

            _isBusy = false;
        }

        private void OnStartGameButtonClicked()
        {
            _ = StartGame();
        }
        private void OnPlayerCountChanged(int playerCount)
        {
            UpdatePlayerCount(playerCount);
        }

        private void UpdatePlayerCount(int playerCount)
        {
            if (playerCountText == null)
                return;

            playerCountText.text =
                $"PLAYERS: {playerCount} / " +
                $"{_lobbyService.MaxPlayers}";
        }

        private void OnCreateButtonClicked()
        {
            if (!SavePlayerName())
                return;

            _ = CreateLobby();
        }

        private void OnJoinButtonClicked()
        {
            if (!SavePlayerName())
                return;

            _ = JoinLobby();
        }

        private void OnLeaveButtonClicked()
        {
            _ = LeaveLobby();
        }

        private async Task CreateLobby()
        {
            if (_isBusy)
                return;

            _isBusy = true;

            SetStatus("Creating lobby...");

            RefreshUI();

            bool success = await _lobbyService.CreateLobby(2);

            if (success)
            {
                string joinCode = _lobbyService.JoinCode;

                if (joinCodeInput != null)
                {
                    joinCodeInput.text = joinCode;
                }

                GUIUtility.systemCopyBuffer = joinCode;

                SetStatus("Lobby created successfully!");

                Debug.Log(
                    $"[LOBBY UI] Created | " +
                    $"Code: {joinCode}"
                );
            }
            else
            {
                SetStatus("Failed to create lobby.");
            }

            _isBusy = false;

            RefreshUI();
        }

        private async Task JoinLobby()
        {
            if (_isBusy)
                return;

            string joinCode =
                joinCodeInput.text.Trim();

            if (string.IsNullOrWhiteSpace(
                    joinCode))
            {
                SetStatus(
                    "Please enter a join code."
                );

                return;
            }

            _isBusy = true;

            SetStatus(
                "Joining lobby..."
            );

            RefreshUI();

            bool success =
                await _lobbyService.JoinLobby(
                    joinCode
                );

            if (success)
            {
                SetStatus(
                    "Successfully joined the lobby!"
                );

                Debug.Log(
                    $"[LOBBY UI] Joined | " +
                    $"Code: {_lobbyService.JoinCode}"
                );
            }
            else
            {
                SetStatus(
                    "Failed to join lobby."
                );
            }

            _isBusy = false;

            RefreshUI();
        }

        private async Task LeaveLobby()
        {
            if (_isBusy)
                return;

            _isBusy = true;

            SetStatus(
                "Leaving lobby..."
            );

            RefreshUI();

            await _lobbyService.LeaveLobby();

            if (joinCodeInput != null)
            {
                joinCodeInput.text =
                    string.Empty;
            }

            SetStatus(
                "You left the lobby."
            );

            _isBusy = false;

            RefreshUI();
        }

        private void RefreshUI()
        {
            bool hasLobby =
                _lobbyService != null &&
                !string.IsNullOrEmpty(
                    _lobbyService.JoinCode
                );

            if (readyButton != null)
            {
                readyButton.interactable =
                    !_isBusy &&
                    _lobbyService.HasLobby;
            }

            if (createButton != null)
            {
                createButton.interactable =
                    !_isBusy && !hasLobby;
            }

            if (joinButton != null)
            {
                joinButton.interactable =
                    !_isBusy && !hasLobby;
            }

            if (leaveButton != null)
            {
                leaveButton.gameObject.SetActive(
                    hasLobby
                );

                leaveButton.interactable =
                    !_isBusy;
            }
            if (startGameButton != null)
            {
                bool canStart =
                    hasLobby &&
                    _lobbyService.IsHost &&
                    _lobbyService.AreAllPlayersReady();

                startGameButton.gameObject.SetActive(
                    _lobbyService.IsHost
                );

                startGameButton.interactable =
                    !_isBusy && canStart;
            }
            if (sessionCodeText != null)
            {
                sessionCodeText.text =
                    hasLobby
                        ? $"JOIN CODE: {_lobbyService.JoinCode}"
                        : "JOIN CODE: -";
            }

            if (hostStatusText != null)
            {
                if (!hasLobby)
                {
                    hostStatusText.text =
                        "NOT CONNECTED";
                }
                else if (_lobbyService.IsHost)
                {
                    hostStatusText.text =
                        "YOU ARE HOST";
                }
                else
                {
                    hostStatusText.text =
                        "YOU ARE CLIENT";
                }
            }

            UpdatePlayerCount(
                _lobbyService.PlayerCount
            );
        }

        
       
        private float _refreshTimer;

        private void Update()
        {
            if (_lobbyService == null)
                return;

            if (!_lobbyService.HasLobby)
                return;

            _refreshTimer += Time.deltaTime;

            if (_refreshTimer < 0.25f)
                return;

            _refreshTimer = 0f;

            RefreshLobbyUI();
        }
        private void RefreshLobbyUI()
        {
            if (_lobbyService == null)
                return;

            bool allReady =
                _lobbyService.AreAllPlayersReady();


            RefreshUI();
        }

        private void SetStatus(
            string message)
        {
            if (statusText != null)
            {
                statusText.text =
                    message;
            }

            Debug.Log(
                $"[LOBBY UI] {message}"
            );
        }

        public void ToggleLobbyPanel()
        {
            lobbyPanel.SetActive(
                !lobbyPanel.activeSelf
            );
        }

        public async void OnReadyButtonClicked()
        {
            if (_isBusy)
                return;

            if (!_lobbyService.HasLobby)
                return;

            _isBusy = true;

            await _lobbyService.ToggleReady();

            _isBusy = false;
        }

        private void UpdateReadyUI(
            bool isReady)
        {
            if (readyText != null)
            {
                readyText.text =
                    isReady
                        ? "READY ✓"
                        : "READY";
            }
        }
        private void OnEnable()
        {
            if (_lobbyService == null)
                return;

            _lobbyService.ReadyChanged += OnReadyChanged;
        }

        private void OnDisable()
        {
            if (_lobbyService == null)
                return;

            _lobbyService.ReadyChanged -= OnReadyChanged;
        }
        private void OnReadyChanged(
                   bool isReady)
        {
            UpdateReadyUI(isReady);

            RefreshUI();

            Debug.Log(
                $"[LOBBY UI] Ready changed | " +
                $"Ready: {isReady} | " +
                $"All Ready: {_lobbyService.AreAllPlayersReady()}"
            );
        }

        private bool SavePlayerName()
        {
            if (_playerNameService == null)
            {
                Debug.LogError(
                    "[LOBBY UI] PlayerNameService is NULL!"
                );

                return false;
            }

            if (playerNameInput == null)
            {
                Debug.LogError(
                    "[LOBBY UI] Player Name Input is NULL!"
                );

                return false;
            }

            string playerName =
                playerNameInput.text.Trim();

            if (string.IsNullOrWhiteSpace(playerName))
            {
                SetStatus(
                    "Please enter your player name."
                );

                return false;
            }

            _playerNameService.SetPlayerName(
                playerName
            );

            Debug.Log(
                $"[LOBBY UI] Player name saved | " +
                $"Name={_playerNameService.GetPlayerName()}"
            );

            return true;
        }
    }
}

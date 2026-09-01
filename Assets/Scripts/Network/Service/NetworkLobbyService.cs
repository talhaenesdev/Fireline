using FireLine.Scripts.Services;
using System;
using System.Threading.Tasks;
using Unity.Services.Multiplayer;
using UnityEngine;

namespace FireLine.Scripts.Network.Service
{
    public class NetworkLobbyService
    {
        private bool _isReady;
        private bool _isQuitting;
        public bool IsReady => _isReady;

        public event Action<bool> ReadyChanged;
        private ISession _session;

        public event Action<int> PlayerCountChanged;
        public event Action LobbyChanged;
        public event Action PlayersUpdated;
        public event Action GameStarting;
        public void OnApplicationQuit()
        {
            _isQuitting = true;
        }
        public bool HasLobby =>
            _session != null;

        public bool IsHost =>
            _session != null &&
            _session.IsHost;

        public string JoinCode =>
            _session?.Code;

        public int PlayerCount =>
            _session?.Players.Count ?? 0;
        public ISession Session =>
            _session;
        public int MaxPlayers =>
            _session?.MaxPlayers ?? 0;
        public bool CanStartGame()
        {
            if (_session == null)
                return false;

            if (!_session.IsHost)
                return false;

            return AreAllPlayersReady();
        }
        public async Task LeaveLobby()
        {
            if (_isQuitting)
                return;

            if (_session == null)
                return;

            try
            {
                await _session.LeaveAsync();

                Debug.Log(
                    "[LOBBY] Left lobby."
                );
            }
            catch (Exception exception)
            {
                if (!_isQuitting)
                {
                    Debug.LogError(
                        $"[LOBBY] Leave failed | {exception}"
                    );
                }
            }

            _session = null;

            NotifyLobbyChanged();
        }

        public bool StartGame()
        {
            if (!CanStartGame())
            {
                Debug.LogWarning(
                    "[LOBBY] Cannot start game."
                );

                return false;
            }

            Debug.Log(
                "[LOBBY] Starting game..."
            );

            return true;
        }
        public async Task<bool> CreateLobby(
            int maxPlayers = 2)
        {
            try
            {
                if (!await EnsureServicesReady())
                    return false;

                if (_session != null)
                {
                    Debug.LogWarning(
                        "[LOBBY] Session already exists."
                    );

                    return false;
                }

                SessionOptions options =
                    new SessionOptions
                    {
                        MaxPlayers = maxPlayers
                    };

                _session =
                    await MultiplayerService.Instance
                        .CreateSessionAsync(options);

                Debug.Log(
                    $"[LOBBY] Lobby created | " +
                    $"Code: {_session.Code}"
                );

                NotifyLobbyChanged();

                LogPlayers();

                return true;
            }
            catch (Exception exception)
            {
                Debug.LogError(
                    $"[LOBBY] Create failed | " +
                    $"{exception}"
                );

                return false;
            }
        }

        public async Task<bool> JoinLobby(
            string joinCode)
        {
            try
            {
                if (!await EnsureServicesReady())
                    return false;

                if (string.IsNullOrWhiteSpace(joinCode))
                {
                    Debug.LogError(
                        "[LOBBY] Join code is empty."
                    );

                    return false;
                }

                _session =
                    await MultiplayerService.Instance
                        .JoinSessionByCodeAsync(
                            joinCode.Trim()
                        );

                Debug.Log(
                    $"[LOBBY] Joined lobby | " +
                    $"Code: {_session.Code}"
                );

                NotifyLobbyChanged();

                LogPlayers();

                return true;
            }
            catch (Exception exception)
            {
                Debug.LogError(
                    $"[LOBBY] Join failed | " +
                    $"{exception}"
                );

                return false;
            }
        }



        private void NotifyLobbyChanged()
        {
            PlayerCountChanged?.Invoke(
                PlayerCount
            );

            PlayersUpdated?.Invoke();

            LobbyChanged?.Invoke();
        }

        private void LogPlayers()
        {
            if (_session == null)
                return;

            Debug.Log(
                $"[LOBBY] Players: {_session.Players.Count}"
            );

            foreach (IReadOnlyPlayer player in _session.Players)
            {
                bool ready = IsPlayerReady(player);

                Debug.Log(
                    $"[LOBBY] Player: {player.Id} | " +
                    $"Ready: {ready}"
                );
            }
        }

        private async Task<bool> EnsureServicesReady()
        {
            UnityServicesInitializer initializer =
                UnityEngine.Object.FindFirstObjectByType<
                    UnityServicesInitializer>();

            if (initializer == null)
            {
                Debug.LogError(
                    "[LOBBY] UnityServicesInitializer " +
                    "not found in scene!"
                );

                return false;
            }

            if (initializer.IsReady)
                return true;

            Debug.Log(
                "[LOBBY] Waiting for Unity Services..."
            );

            bool initialized =
                await initializer.Initialize();

            if (!initialized)
            {
                Debug.LogError(
                    "[LOBBY] Unity Services initialization failed."
                );

                return false;
            }

            return true;
        }

        public async Task ToggleReady()
        {
            if (_session == null)
            {
                Debug.LogWarning(
                    "[LOBBY] Cannot change ready state. No session."
                );

                return;
            }

            bool previousReady = _isReady;

            _isReady = !_isReady;

            try
            {
                PlayerProperty property =
                    new PlayerProperty(
                        _isReady.ToString()
                    );

                _session.CurrentPlayer.SetProperty(
                    "Ready",
                    property
                );

                await _session.SaveCurrentPlayerDataAsync();

                Debug.Log(
                    $"[LOBBY] Ready saved | " +
                    $"Ready: {_isReady}"
                );

                ReadyChanged?.Invoke(_isReady);
            }
            catch (Exception exception)
            {
                _isReady = previousReady;

                Debug.LogError(
                    $"[LOBBY] Ready update failed | {exception}"
                );
            }
        }

        public bool IsPlayerReady(IReadOnlyPlayer player)
        {
            if (player == null)
                return false;

            if (!player.Properties.TryGetValue(
                    "Ready",
                    out PlayerProperty property))
            {
                return false;
            }

            return bool.TryParse(
                property.Value,
                out bool ready
            ) && ready;
        }

        private void NotifyPlayersUpdated()
        {
            PlayerCountChanged?.Invoke(
                PlayerCount
            );

            PlayersUpdated?.Invoke();

            LobbyChanged?.Invoke();
        }

        public bool AreAllPlayersReady()
        {
            if (_session == null)
                return false;

            if (_session.Players.Count < _session.MaxPlayers)
                return false;

            foreach (IReadOnlyPlayer player in _session.Players)
            {
                if (!IsPlayerReady(player))
                    return false;
            }

            return true;
        }
        public void DebugPlayers()
        {
            if (_session == null)
            {
                Debug.Log(
                    "[LOBBY] No active session."
                );

                return;
            }

            Debug.Log(
                $"[LOBBY] Players: " +
                $"{_session.Players.Count}/{_session.MaxPlayers}"
            );

            foreach (IReadOnlyPlayer player in _session.Players)
            {
                bool ready = IsPlayerReady(player);

                Debug.Log(
                    $"[LOBBY] Player | " +
                    $"Id: {player.Id} | " +
                    $"Ready: {ready}"
                );
            }

            Debug.Log(
                $"[LOBBY] ALL READY: " +
                $"{AreAllPlayersReady()}"
            );
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlowIoC.BaseModule.Injectable.Attributes;
using FlowIoC.ConsoleModule;
using Modules.NetworkSessionModule.Shared.Data.ValueObjects;
using Modules.NetworkSessionModule.Signals;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Multiplayer;
using UnityEngine.SceneManagement;

namespace Modules.NetworkSessionModule.Services
{
    internal class NetworkSessionService : INetworkSessionService
    {
        private const string READY_PROPERTY = "Ready";

        [InjectSignal] private NetworkSessionSignals _signals { get; set; }

        private ISession _session;
        private NetworkManager _hookedManager;

        private static NetworkManager Manager => NetworkManager.Singleton;

        // The session outlives the scene this service was bound in, so the service of a later
        // scene finds it in Multiplayer Services rather than in a field.
        private ISession Session
        {
            get
            {
                if (_session != null)
                    return _session;

                if (UnityServices.State != ServicesInitializationState.Initialized)
                    return null;

                _session = MultiplayerService.Instance.Sessions.Values.FirstOrDefault();
                if (_session != null)
                {
                    _session.Changed += OnSessionChanged;
                    _session.SessionPropertiesChanged += OnSessionChanged;
                }

                return _session;
            }
        }

        public SessionStateVO State
        {
            get
            {
                ISession session = Session;
                if (session == null)
                    return SessionStateVO.None;

                // Ready means everybody who is in, however many that is; a full lobby is not required.
                bool allReady = session.Players.Count > 0 && session.Players.All(IsReady);
                return new SessionStateVO(true, session.IsHost, session.Code, session.Players.Count,
                    session.MaxPlayers, IsReady(session.CurrentPlayer), allReady);
            }
        }

        public bool IsServer => Manager != null && Manager.IsServer;
        public bool IsClient => Manager != null && Manager.IsClient;
        public bool IsListening => Manager != null && Manager.IsListening;
        public ulong LocalClientId => Manager != null ? Manager.LocalClientId : 0;

        public IReadOnlyList<ulong> ConnectedClientIds =>
            Manager != null && Manager.IsServer ? Manager.ConnectedClientsIds : Array.Empty<ulong>();

        public async Task<bool> InitializeAsync()
        {
            try
            {
                if (UnityServices.State != ServicesInitializationState.Initialized)
                    await UnityServices.InitializeAsync();

                if (!AuthenticationService.Instance.IsSignedIn)
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();

                return true;
            }
            catch (Exception exception)
            {
                FlowLogger.LogError($"NetworkSessionService - Unity Services could not start: {exception.Message}");
                return false;
            }
        }

        public async Task<bool> CreateSessionAsync(int maxPlayers)
        {
            if (Session != null || !await InitializeAsync())
                return false;

            try
            {
                Adopt(await MultiplayerService.Instance.CreateSessionAsync(new SessionOptions { MaxPlayers = maxPlayers }));
                return true;
            }
            catch (Exception exception)
            {
                FlowLogger.LogError($"NetworkSessionService - the session could not be created: {exception.Message}");
                return false;
            }
        }

        public async Task<bool> JoinSessionAsync(string joinCode)
        {
            if (string.IsNullOrWhiteSpace(joinCode) || !await InitializeAsync())
                return false;

            try
            {
                Adopt(await MultiplayerService.Instance.JoinSessionByCodeAsync(joinCode.Trim()));
                return true;
            }
            catch (Exception exception)
            {
                FlowLogger.LogError($"NetworkSessionService - the session could not be joined: {exception.Message}");
                return false;
            }
        }

        public async Task LeaveSessionAsync()
        {
            ISession session = Session;
            if (session == null)
                return;

            session.Changed -= OnSessionChanged;
            session.SessionPropertiesChanged -= OnSessionChanged;
            _session = null;

            try
            {
                await session.LeaveAsync();
            }
            catch (Exception exception)
            {
                FlowLogger.LogWarning($"NetworkSessionService - leaving the session failed: {exception.Message}");
            }

            _signals.Outgoing.SessionChanged.Dispatch(State);
        }

        public async Task<bool> SetReadyAsync(bool isReady)
        {
            ISession session = Session;
            if (session == null)
                return false;

            try
            {
                session.CurrentPlayer.SetProperty(READY_PROPERTY, new PlayerProperty(isReady.ToString()));
                await session.SaveCurrentPlayerDataAsync();
                _signals.Outgoing.SessionChanged.Dispatch(State);
                return true;
            }
            catch (Exception exception)
            {
                FlowLogger.LogError($"NetworkSessionService - the ready flag could not be saved: {exception.Message}");
                return false;
            }
        }

        public async Task<bool> SetSessionPropertyAsync(string key, string value)
        {
            ISession session = Session;
            if (session == null || !session.IsHost)
                return false;

            try
            {
                IHostSession host = session.AsHost();
                host.SetProperty(key, new SessionProperty(value, VisibilityPropertyOptions.Member));
                await host.SavePropertiesAsync();
                _signals.Outgoing.SessionChanged.Dispatch(State);
                return true;
            }
            catch (Exception exception)
            {
                FlowLogger.LogError($"NetworkSessionService - the session property '{key}' could not be saved: {exception.Message}");
                return false;
            }
        }

        public string GetSessionProperty(string key)
        {
            ISession session = Session;
            return session != null && session.Properties.TryGetValue(key, out SessionProperty property) ? property.Value : null;
        }

        public async Task<bool> StartHostNetworkAsync()
        {
            ISession session = Session;
            if (session == null || !session.IsHost)
                return false;

            try
            {
                await session.AsHost().Network.StartRelayNetworkAsync(RelayNetworkOptions.Default);
                return Manager != null && Manager.IsServer;
            }
            catch (Exception exception)
            {
                FlowLogger.LogError($"NetworkSessionService - the Relay network could not start: {exception.Message}");
                return false;
            }
        }

        public bool LoadNetworkScene(string sceneName)
        {
            if (Manager == null || !Manager.IsServer || Manager.SceneManager == null)
                return false;

            return Manager.SceneManager.LoadScene(sceneName, LoadSceneMode.Single) == SceneEventProgressStatus.Started;
        }

        public void Shutdown()
        {
            if (Manager != null && Manager.IsListening)
                Manager.Shutdown();
        }

        public void HookNetworkManager()
        {
            if (_hookedManager != null || Manager == null)
                return;

            _hookedManager = Manager;
            _hookedManager.OnServerStarted += OnServerStarted;
            _hookedManager.OnClientConnectedCallback += OnClientConnected;
            _hookedManager.OnClientDisconnectCallback += OnClientDisconnected;
        }

        public void UnhookNetworkManager()
        {
            if (_session != null)
            {
                _session.Changed -= OnSessionChanged;
                _session.SessionPropertiesChanged -= OnSessionChanged;
            }

            if (_hookedManager == null)
                return;

            _hookedManager.OnServerStarted -= OnServerStarted;
            _hookedManager.OnClientConnectedCallback -= OnClientConnected;
            _hookedManager.OnClientDisconnectCallback -= OnClientDisconnected;
            _hookedManager = null;
        }

        private void Adopt(ISession session)
        {
            _session = session;
            _session.Changed += OnSessionChanged;
            _session.SessionPropertiesChanged += OnSessionChanged;
            _signals.Outgoing.SessionChanged.Dispatch(State);
        }

        private void OnSessionChanged() => _signals.Outgoing.SessionChanged.Dispatch(State);

        private void OnServerStarted() => _signals.Outgoing.ServerStarted.Dispatch();

        private void OnClientConnected(ulong clientId)
        {
            if (IsServer)
                _signals.Outgoing.ClientConnected.Dispatch(clientId);
        }

        private void OnClientDisconnected(ulong clientId)
        {
            if (IsServer)
                _signals.Outgoing.ClientDisconnected.Dispatch(clientId);
        }

        private static bool IsReady(IReadOnlyPlayer player) =>
            player != null
            && player.Properties.TryGetValue(READY_PROPERTY, out PlayerProperty property)
            && bool.TryParse(property.Value, out bool ready)
            && ready;
    }
}

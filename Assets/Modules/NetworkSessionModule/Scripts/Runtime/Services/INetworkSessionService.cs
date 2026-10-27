using System.Collections.Generic;
using System.Threading.Tasks;
using Modules.NetworkSessionModule.Shared.Data.ValueObjects;

namespace Modules.NetworkSessionModule.Services
{
    /// <summary>
    /// Unity Gaming Services, Multiplayer Sessions, Relay and Netcode's NetworkManager behind one
    /// surface. Every async member answers false or nothing on failure and logs why; none throws.
    /// </summary>
    public partial interface INetworkSessionService
    {
        SessionStateVO State { get; }

        bool IsServer { get; }
        bool IsClient { get; }
        bool IsListening { get; }
        ulong LocalClientId { get; }
        IReadOnlyList<ulong> ConnectedClientIds { get; }

        /// <summary>Initialises Unity Services and signs in anonymously, once.</summary>
        Task<bool> InitializeAsync();

        Task<bool> CreateSessionAsync(int maxPlayers);
        Task<bool> JoinSessionAsync(string joinCode);
        Task LeaveSessionAsync();
        Task<bool> SetReadyAsync(bool isReady);

        /// <summary>Host only: stores a value every member of the session can read.</summary>
        Task<bool> SetSessionPropertyAsync(string key, string value);

        /// <summary>A value the host stored on the session, or null.</summary>
        string GetSessionProperty(string key);

        /// <summary>Starts the Relay network as host of the current session.</summary>
        Task<bool> StartHostNetworkAsync();

        /// <summary>Loads a scene on every connected client through Netcode's scene manager. Server only.</summary>
        bool LoadNetworkScene(string sceneName);

        /// <summary>Stops Netcode if it is running.</summary>
        void Shutdown();

        /// <summary>Starts forwarding NetworkManager's callbacks as this module's Outgoing signals.</summary>
        void HookNetworkManager();

        void UnhookNetworkManager();

        public static partial class Commands
        {
        }
    }
}

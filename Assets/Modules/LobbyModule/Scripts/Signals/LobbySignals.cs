using FlowIoC.BaseModule.Signals;
using Modules.LobbyModule.Shared.Data.ValueObjects;
using Modules.NetworkSessionModule.Shared.Data.ValueObjects;

namespace Modules.LobbyModule.Signals
{
    public class LobbySignals : ISignalHolder
    {
        public LobbySignalsIncoming Incoming = new();
        public LobbySignalsOutgoing Outgoing = new();
    }

    public class LobbySignalsIncoming
    {
        /// <summary>The player name typed in the panel.</summary>
        public Signal<string> CreateLobby = new();

        /// <summary>The player name, then the join code.</summary>
        public Signal<string, string> JoinLobby = new();

        public Signal LeaveLobby = new();
        public Signal ToggleReady = new();
        public Signal StartGame = new();

        /// <summary>The host moves the lobby on to the next game mode.</summary>
        public Signal CycleGameMode = new();

        /// <summary>The next difficulty for a zombie match.</summary>
        public Signal CycleDifficulty = new();

        /// <summary>The next map for a zombie match.</summary>
        public Signal CycleMap = new();

        /// <summary>The network session changed under the lobby.</summary>
        public Signal<SessionStateVO> SessionChanged = new();
    }

    public class LobbySignalsOutgoing
    {
        public Signal<LobbyStateVO> StateChanged = new();
    }
}

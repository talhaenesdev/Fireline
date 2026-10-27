using FlowIoC.BaseModule.Signals;
using Modules.MatchModule.Shared.Data.ValueObjects;

namespace Modules.MatchModule.Signals
{
    public class MatchSignals : ISignalHolder
    {
        public MatchSignalsIncoming Incoming = new();
        public MatchSignalsOutgoing Outgoing = new();
    }

    public class MatchSignalsIncoming
    {
        public Signal<ulong> ClientConnected = new();
        public Signal<ulong> ClientDisconnected = new();

        /// <summary>Victim, then killer.</summary>
        public Signal<ulong, ulong> PlayerDied = new();

        public Signal<ulong, string> PlayerNameChanged = new();

        public Signal ExitMatch = new();

        /// <summary>A player killed a zombie. Server only.</summary>
        public Signal<ulong> ZombieKilled = new();
    }

    public class MatchSignalsOutgoing
    {
        /// <summary>The match scene is up on this process.</summary>
        public Signal Started = new();

        public Signal<ScoreEntryVO[]> ScoresChanged = new();

        /// <summary>The match is a zombie match: the waves begin. Server only.</summary>
        public Signal ZombieWavesStarted = new();

        /// <summary>Whether players' rounds hurt other players. Server only.</summary>
        public Signal<bool> FriendlyFireChanged = new();
    }
}

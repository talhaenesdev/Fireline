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

        /// <summary>A downed player of a zombie match is back up. Server only.</summary>
        public Signal<ulong> PlayerRevived = new();

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

        /// <summary>How hard the zombie match is, before its waves begin. Server only.</summary>
        public Signal<int> ZombieDifficultyChosen = new();

        /// <summary>The match is a zombie match: the waves begin. Server only.</summary>
        public Signal ZombieWavesStarted = new();

        /// <summary>Every player of a zombie match is down. On every client.</summary>
        public Signal GameOver = new();

        /// <summary>Whether players' rounds hurt other players. Server only.</summary>
        public Signal<bool> FriendlyFireChanged = new();
    }
}

using Modules.MatchModule.Shared.Data.ValueObjects;

namespace Modules.MatchModule.Entities
{
    /// <summary>The replicated scoreboard. The server writes rows; every client reads them.</summary>
    internal interface IScoreboard
    {
        bool IsServer { get; }
        void SetEntryServer(ScoreEntryVO entry);
        void RemoveEntryServer(ulong clientId);
        ScoreEntryVO[] ToEntries();

        /// <summary>Server only: every player is down, and every client is told the match is over.</summary>
        void SetGameOverServer();
    }
}

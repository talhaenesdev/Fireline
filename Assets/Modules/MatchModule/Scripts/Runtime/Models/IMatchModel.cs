using System.Collections.Generic;
using Modules.MatchModule.Entities;
using Modules.MatchModule.Shared.Constants;
using Modules.MatchModule.Shared.Data.ValueObjects;
using Modules.MatchModule.Shared.Enums;

namespace Modules.MatchModule.Models
{
    /// <summary>The server's record of the match: the arena, the scoreboard it writes, and each player's score.</summary>
    internal interface IMatchModel
    {
        IMatchArena Arena { get; set; }
        IScoreboard Scoreboard { get; set; }

        /// <summary>
        /// Every Root in the scene has set up, so the connectors carry what a spawned player announces.
        /// </summary>
        bool IsLaunched { get; set; }

        GameMode Mode { get; set; }

        ScoreEntryVO GetScore(ulong clientId);
        void SetScore(ScoreEntryVO entry);
        void RemoveScore(ulong clientId);
    }

    internal class MatchModel : IMatchModel
    {
        private readonly Dictionary<ulong, ScoreEntryVO> _scores = new();

        public IMatchArena Arena { get; set; }
        public IScoreboard Scoreboard { get; set; }
        public bool IsLaunched { get; set; }
        public GameMode Mode { get; set; }

        public ScoreEntryVO GetScore(ulong clientId) =>
            _scores.TryGetValue(clientId, out ScoreEntryVO entry)
                ? entry
                : new ScoreEntryVO(clientId, MatchConstants.DEFAULT_PLAYER_NAME, 0, 0);

        public void SetScore(ScoreEntryVO entry) => _scores[entry.ClientId] = entry;

        public void RemoveScore(ulong clientId) => _scores.Remove(clientId);
    }
}

using System;
using Modules.MatchModule.Shared.Data.ValueObjects;

namespace Modules.MatchModule.MatchScreenModule.Models
{
    /// <summary>The last scoreboard reported, for a screen that opens after it arrived.</summary>
    internal interface IMatchScreenModel
    {
        ScoreEntryVO[] Scores { get; set; }
    }

    internal class MatchScreenModel : IMatchScreenModel
    {
        public ScoreEntryVO[] Scores { get; set; } = Array.Empty<ScoreEntryVO>();
    }
}

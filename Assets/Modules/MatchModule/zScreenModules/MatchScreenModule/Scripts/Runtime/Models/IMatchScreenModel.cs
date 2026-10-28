using System;
using Modules.MatchModule.Shared.Data.ValueObjects;
using Modules.PlayerModule.Shared.Data.ValueObjects;

namespace Modules.MatchModule.MatchScreenModule.Models
{
    /// <summary>The last scoreboard and camera settings reported, for a screen that opens after they arrived.</summary>
    internal interface IMatchScreenModel
    {
        ScoreEntryVO[] Scores { get; set; }
        CameraSettingsVO? CameraSettings { get; set; }
    }

    internal class MatchScreenModel : IMatchScreenModel
    {
        public ScoreEntryVO[] Scores { get; set; } = Array.Empty<ScoreEntryVO>();
        public CameraSettingsVO? CameraSettings { get; set; }
    }
}

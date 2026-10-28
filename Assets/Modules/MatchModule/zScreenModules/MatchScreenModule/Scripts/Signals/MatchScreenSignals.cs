using FlowIoC.BaseModule.Signals;
using Modules.MatchModule.Shared.Data.ValueObjects;
using Modules.PlayerModule.Shared.Data.ValueObjects;

namespace Modules.MatchModule.MatchScreenModule.Signals
{
    public class MatchScreenSignals : ISignalHolder
    {
        public MatchScreenSignalsIncoming Incoming = new();
        public MatchScreenSignalsOutgoing Outgoing = new();
    }

    public class MatchScreenSignalsIncoming
    {
        public Signal Open = new();
        public Signal<ScoreEntryVO[]> ApplyScores = new();

        /// <summary>Everybody is down: the match is over.</summary>
        public Signal ShowGameOver = new();

        /// <summary>The camera settings and their ranges, for the settings panel.</summary>
        public Signal<CameraSettingsVO> ApplyCameraSettings = new();
    }

    public class MatchScreenSignalsOutgoing
    {
        public Signal ExitClicked = new();

        /// <summary>A player moved a camera setting: height, follow speed, shake.</summary>
        public Signal<float, float, float> CameraSettingsEdited = new();
    }
}

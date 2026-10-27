using FlowIoC.BaseModule.Signals;
using Modules.MatchModule.Shared.Data.ValueObjects;

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
    }

    public class MatchScreenSignalsOutgoing
    {
        public Signal ExitClicked = new();
    }
}

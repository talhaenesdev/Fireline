using FlowIoC.BaseModule.Signals;
using Modules.MatchModule.Entities;

namespace Modules.MatchModule.Signals
{
    internal class MatchInternalSignals : ISignalHolder
    {
        public Signal Launched = new();
        public Signal<IMatchArena> ArenaRegistered = new();
        public Signal<IScoreboard> ScoreboardRegistered = new();
    }
}

using FlowIoC.BaseModule.Controller;
using FlowIoC.BaseModule.Injectable.Attributes;
using Modules.MatchModule.Entities;
using Modules.MatchModule.Models;

namespace Modules.MatchModule.Controllers
{
    internal class StoreArenaCommand : Command
    {
        [Inject] private IMatchModel _model { get; set; }

        [SignalParam] private IMatchArena _arena { get; set; }

        public override void Execute() => _model.Arena = _arena;
    }

    internal class MarkLaunchedCommand : Command
    {
        [Inject] private IMatchModel _model { get; set; }

        public override void Execute() => _model.IsLaunched = true;
    }

    internal class StoreScoreboardCommand : Command
    {
        [Inject] private IMatchModel _model { get; set; }

        [SignalParam] private IScoreboard _scoreboard { get; set; }

        public override void Execute() => _model.Scoreboard = _scoreboard;
    }
}

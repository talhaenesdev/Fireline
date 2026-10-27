using FlowIoC.BaseModule.Controller;
using FlowIoC.BaseModule.Injectable.Attributes;
using Modules.MatchModule.Models;
using Modules.MatchModule.Shared.Data.ValueObjects;
using Modules.NetworkSessionModule.Services;

namespace Modules.MatchModule.Controllers
{
    /// <summary>
    /// The players who came over from the lobby connected before this scene existed, so no
    /// ClientConnected put them on the board. They go on it when the board appears.
    /// </summary>
    internal class AddConnectedScoresCommand : Command
    {
        [Inject] private IMatchModel _model { get; set; }
        [Inject] private INetworkSessionService _session { get; set; }

        public override void Execute()
        {
            foreach (ulong clientId in _session.ConnectedClientIds)
            {
                ScoreEntryVO entry = _model.GetScore(clientId);
                _model.SetScore(entry);
                _model.Scoreboard.SetEntryServer(entry);
            }
        }
    }
}

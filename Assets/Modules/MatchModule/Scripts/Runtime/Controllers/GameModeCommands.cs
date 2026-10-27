using System;
using FlowIoC.BaseModule.Controller;
using FlowIoC.BaseModule.Injectable.Attributes;
using Modules.MatchModule.Models;
using Modules.MatchModule.Shared.Constants;
using Modules.MatchModule.Shared.Enums;
using Modules.MatchModule.Signals;
using Modules.NetworkSessionModule.Services;

namespace Modules.MatchModule.Controllers
{
    /// <summary>The host chose the mode in the lobby; the session carried it here. Deathmatch when nothing was chosen.</summary>
    internal class ReadGameModeCommand : Command
    {
        [Inject] private IMatchModel _model { get; set; }
        [Inject] private INetworkSessionService _session { get; set; }

        public override void Execute() =>
            _model.Mode = Enum.TryParse(_session.GetSessionProperty(MatchConstants.GAME_MODE_PROPERTY), out GameMode mode)
                ? mode
                : GameMode.Deathmatch;
    }

    /// <summary>
    /// What the mode means for the match. Deathmatch is players against players. A zombie match
    /// puts the players on one side, so their rounds stop hurting each other, and starts the waves.
    /// </summary>
    internal class StartGameModeCommand : Command
    {
        [Inject] private IMatchModel _model { get; set; }
        [InjectSignal] private MatchSignals _signals { get; set; }

        public override void Execute()
        {
            bool isZombie = _model.Mode == GameMode.Zombie;
            _signals.Outgoing.FriendlyFireChanged.Dispatch(!isZombie);

            if (isZombie)
                _signals.Outgoing.ZombieWavesStarted.Dispatch();
        }
    }
}

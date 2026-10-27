using FlowIoC.BaseModule.Controller;
using FlowIoC.BaseModule.Injectable.Attributes;
using Modules.LobbyModule.Models;
using Modules.LobbyModule.Shared.Constants;
using Modules.LobbyModule.Signals;
using Modules.MatchModule.Shared.Enums;
using Modules.NetworkSessionModule.Services;

namespace Modules.LobbyModule.Controllers
{
    /// <summary>Netcode carries every connected client into the match scene with the host; each mode has its own map.</summary>
    internal class LoadGameSceneCommand : Command
    {
        [Inject] private ILobbyModel _model { get; set; }
        [Inject] private INetworkSessionService _session { get; set; }
        [InjectSignal] private LobbySignals _signals { get; set; }

        public override void Execute()
        {
            string scene = _model.Mode == GameMode.Zombie ? LobbyConstants.ZOMBIE_SCENE : LobbyConstants.GAME_SCENE;

            if (_session.LoadNetworkScene(scene))
                return;

            Retain();
            this.StopWith(_model, _signals, LobbyConstants.STATUS_START_FAILED);
        }
    }
}

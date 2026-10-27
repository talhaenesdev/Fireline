using FlowIoC.BaseModule.Controller;
using FlowIoC.BaseModule.Injectable.Attributes;
using Modules.LobbyModule.Models;
using Modules.LobbyModule.Signals;
using Modules.NetworkSessionModule.Services;
using Modules.NetworkSessionModule.Shared.Data.ValueObjects;

namespace Modules.LobbyModule.Controllers
{
    /// <summary>Only the host starts, and only once the lobby is full and everybody in it is ready.</summary>
    internal class CheckCanStartCommand : Command
    {
        [Inject] private ILobbyModel _model { get; set; }
        [Inject] private INetworkSessionService _session { get; set; }
        [InjectSignal] private LobbySignals _signals { get; set; }

        public override void Execute()
        {
            SessionStateVO state = _session.State;
            _model.SetSession(state);

            if (state.HasSession && state.IsHost && state.AreAllReady)
                return;

            Retain();
            this.StopWith(_model, _signals, null);
        }
    }
}

using FlowIoC.BaseModule.Controller;
using FlowIoC.BaseModule.Injectable.Attributes;
using Modules.LobbyModule.Models;
using Modules.LobbyModule.Signals;
using Modules.NetworkSessionModule.Shared.Data.ValueObjects;

namespace Modules.LobbyModule.Controllers
{
    /// <summary>Another player joined, left or readied: the session changed without the lobby asking.</summary>
    internal class StoreSessionCommand : Command
    {
        [Inject] private ILobbyModel _model { get; set; }
        [InjectSignal] private LobbySignals _signals { get; set; }

        [SignalParam] private SessionStateVO _session { get; set; }

        public override void Execute()
        {
            _model.SetSession(_session);
            _signals.Outgoing.StateChanged.Dispatch(_model.ToStateVO());
        }
    }
}

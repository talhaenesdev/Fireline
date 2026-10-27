using FlowIoC.BaseModule.Controller;
using FlowIoC.BaseModule.Injectable.Attributes;
using Modules.LobbyModule.Models;
using Modules.LobbyModule.Signals;

namespace Modules.LobbyModule.Controllers
{
    /// <summary>The last step of a lobby operation that succeeded: frees the lobby and reports.</summary>
    internal class FinishLobbyCommand : Command<string>
    {
        [Inject] private ILobbyModel _model { get; set; }
        [InjectSignal] private LobbySignals _signals { get; set; }

        public override void Execute(string status)
        {
            _model.Finish(status);
            _signals.Outgoing.StateChanged.Dispatch(_model.ToStateVO());
        }
    }
}

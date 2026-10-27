using FlowIoC.BaseModule.Controller;
using FlowIoC.BaseModule.Injectable.Attributes;
using Modules.LobbyModule.Models;
using Modules.LobbyModule.Signals;

namespace Modules.LobbyModule.Controllers
{
    /// <summary>
    /// The first step of every lobby operation. A second click while one is running ends its own
    /// flow here, so two requests never race against Multiplayer Services.
    /// </summary>
    internal class ClaimLobbyCommand : Command<string>
    {
        [Inject] private ILobbyModel _model { get; set; }
        [InjectSignal] private LobbySignals _signals { get; set; }

        public override void Execute(string status)
        {
            if (_model.TryClaim(status))
            {
                _signals.Outgoing.StateChanged.Dispatch(_model.ToStateVO());
                return;
            }

            Retain();
            Stop();
        }
    }
}

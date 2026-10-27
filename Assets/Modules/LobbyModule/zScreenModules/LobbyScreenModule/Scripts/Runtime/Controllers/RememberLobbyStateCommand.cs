using FlowIoC.BaseModule.Controller;
using FlowIoC.BaseModule.Injectable.Attributes;
using Modules.LobbyModule.LobbyScreenModule.Models;
using Modules.LobbyModule.Shared.Data.ValueObjects;

namespace Modules.LobbyModule.LobbyScreenModule.Controllers
{
    /// <summary>Keeps the state for the next opening; an open panel redraws from its Mediator.</summary>
    internal class RememberLobbyStateCommand : Command
    {
        [Inject] private ILobbyScreenModel _model { get; set; }

        [SignalParam] private LobbyStateVO _state { get; set; }

        public override void Execute() => _model.State = _state;
    }
}

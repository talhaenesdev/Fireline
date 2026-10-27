using FlowIoC.BaseModule.Controller;
using Modules.LobbyModule.Models;
using Modules.LobbyModule.Signals;

namespace Modules.LobbyModule.Controllers
{
    /// <summary>
    /// How a lobby step that cannot go on ends its flow: the lobby is freed with the reason as its
    /// status, the panel redraws, and the steps behind it do not run. The step has retained already.
    /// </summary>
    internal static class LobbyFailure
    {
        public static void StopWith(this CommandBody command, ILobbyModel model, LobbySignals signals, string status)
        {
            model.Finish(status);
            signals.Outgoing.StateChanged.Dispatch(model.ToStateVO());
            command.Stop();
        }
    }
}

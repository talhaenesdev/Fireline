using System;
using FlowIoC.BaseModule.Controller;
using FlowIoC.BaseModule.Injectable.Attributes;
using FlowIoC.ConsoleModule;
using Modules.LobbyModule.Models;
using Modules.LobbyModule.Shared.Constants;
using Modules.LobbyModule.Signals;
using Modules.NetworkSessionModule.Services;

namespace Modules.LobbyModule.Controllers
{
    /// <summary>Signs in while the player is still on the menu, so the first lobby click does not wait on it.</summary>
    internal class InitializeServicesCommand : Command
    {
        [Inject] private ILobbyModel _model { get; set; }
        [Inject] private INetworkSessionService _session { get; set; }
        [InjectSignal] private LobbySignals _signals { get; set; }

        public override async void Execute()
        {
            Retain();

            try
            {
                bool ready = await _session.InitializeAsync();
                _model.SetSession(_session.State);
                _model.Finish(ready ? LobbyConstants.STATUS_IDLE : LobbyConstants.STATUS_SERVICES_FAILED);
            }
            catch (Exception exception)
            {
                FlowLogger.LogError($"InitializeServicesCommand threw: {exception}");
                _model.Finish(LobbyConstants.STATUS_SERVICES_FAILED);
            }

            _signals.Outgoing.StateChanged.Dispatch(_model.ToStateVO());
            Release();
        }
    }
}

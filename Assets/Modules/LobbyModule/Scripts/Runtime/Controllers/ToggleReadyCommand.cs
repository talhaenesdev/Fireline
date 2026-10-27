using System;
using FlowIoC.BaseModule.Controller;
using FlowIoC.BaseModule.Injectable.Attributes;
using FlowIoC.ConsoleModule;
using Modules.LobbyModule.Models;
using Modules.NetworkSessionModule.Services;

namespace Modules.LobbyModule.Controllers
{
    internal class ToggleReadyCommand : Command
    {
        [Inject] private ILobbyModel _model { get; set; }
        [Inject] private INetworkSessionService _session { get; set; }

        public override async void Execute()
        {
            Retain();

            if (!_model.Session.HasSession)
            {
                _model.Finish(null);
                Stop();
                return;
            }

            try
            {
                await _session.SetReadyAsync(!_model.Session.IsLocalReady);
            }
            catch (Exception exception)
            {
                FlowLogger.LogError($"ToggleReadyCommand threw: {exception}");
            }

            // A flag that failed to save leaves the old state in the session, which is what is drawn.
            _model.SetSession(_session.State);
            Release();
        }
    }
}

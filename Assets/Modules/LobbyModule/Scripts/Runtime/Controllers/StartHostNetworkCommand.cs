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
    internal class StartHostNetworkCommand : Command
    {
        [Inject] private ILobbyModel _model { get; set; }
        [Inject] private INetworkSessionService _session { get; set; }
        [InjectSignal] private LobbySignals _signals { get; set; }

        public override async void Execute()
        {
            Retain();

            try
            {
                if (await _session.StartHostNetworkAsync())
                {
                    Release();
                    return;
                }

                this.StopWith(_model, _signals, LobbyConstants.STATUS_START_FAILED);
            }
            catch (Exception exception)
            {
                FlowLogger.LogError($"StartHostNetworkCommand threw: {exception}");
                this.StopWith(_model, _signals, LobbyConstants.STATUS_START_FAILED);
            }
        }
    }
}

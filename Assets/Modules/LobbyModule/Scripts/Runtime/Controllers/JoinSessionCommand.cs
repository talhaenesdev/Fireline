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
    internal class JoinSessionCommand : Command
    {
        [Inject] private ILobbyModel _model { get; set; }
        [Inject] private INetworkSessionService _session { get; set; }
        [InjectSignal] private LobbySignals _signals { get; set; }

        [SignalParam(1)] private string _joinCode { get; set; }

        public override async void Execute()
        {
            Retain();

            if (string.IsNullOrWhiteSpace(_joinCode))
            {
                this.StopWith(_model, _signals, LobbyConstants.STATUS_CODE_MISSING);
                return;
            }

            try
            {
                if (!await _session.JoinSessionAsync(_joinCode))
                {
                    this.StopWith(_model, _signals, LobbyConstants.STATUS_JOIN_FAILED);
                    return;
                }

                _model.SetSession(_session.State);
                Release();
            }
            catch (Exception exception)
            {
                FlowLogger.LogError($"JoinSessionCommand threw: {exception}");
                this.StopWith(_model, _signals, LobbyConstants.STATUS_JOIN_FAILED);
            }
        }
    }
}

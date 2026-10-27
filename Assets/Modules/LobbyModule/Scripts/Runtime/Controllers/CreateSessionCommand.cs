using System;
using FlowIoC.BaseModule.Controller;
using FlowIoC.BaseModule.Injectable.Attributes;
using FlowIoC.ConsoleModule;
using Modules.LobbyModule.Models;
using Modules.LobbyModule.Shared.Constants;
using Modules.LobbyModule.Signals;
using Modules.NetworkSessionModule.Services;
using UnityEngine;

namespace Modules.LobbyModule.Controllers
{
    internal class CreateSessionCommand : Command
    {
        [Inject] private ILobbyModel _model { get; set; }
        [Inject] private INetworkSessionService _session { get; set; }
        [InjectSignal] private LobbySignals _signals { get; set; }

        public override async void Execute()
        {
            Retain();

            try
            {
                if (!await _session.CreateSessionAsync(LobbyConstants.MAX_PLAYERS))
                {
                    this.StopWith(_model, _signals, LobbyConstants.STATUS_CREATE_FAILED);
                    return;
                }

                _model.SetSession(_session.State);

                // The host reads the code out to the other player, so it is ready to paste.
                GUIUtility.systemCopyBuffer = _session.State.JoinCode;
                Release();
            }
            catch (Exception exception)
            {
                FlowLogger.LogError($"CreateSessionCommand threw: {exception}");
                this.StopWith(_model, _signals, LobbyConstants.STATUS_CREATE_FAILED);
            }
        }
    }
}

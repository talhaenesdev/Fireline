using System;
using FlowIoC.BaseModule.Controller;
using FlowIoC.BaseModule.Injectable.Attributes;
using FlowIoC.ConsoleModule;
using Modules.LobbyModule.Models;
using Modules.NetworkSessionModule.Services;

namespace Modules.LobbyModule.Controllers
{
    /// <summary>Leaving cannot fail from the player's side: whatever the service answers, the lobby is left.</summary>
    internal class LeaveSessionCommand : Command
    {
        [Inject] private ILobbyModel _model { get; set; }
        [Inject] private INetworkSessionService _session { get; set; }

        public override async void Execute()
        {
            Retain();

            try
            {
                await _session.LeaveSessionAsync();
            }
            catch (Exception exception)
            {
                FlowLogger.LogError($"LeaveSessionCommand threw: {exception}");
            }

            _model.SetSession(_session.State);
            Release();
        }
    }
}

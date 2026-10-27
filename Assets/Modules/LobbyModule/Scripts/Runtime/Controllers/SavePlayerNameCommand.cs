using FlowIoC.BaseModule.Controller;
using FlowIoC.BaseModule.Injectable.Attributes;
using Modules.LobbyModule.Models;
using Modules.LobbyModule.Shared.Constants;
using Modules.LobbyModule.Signals;
using Modules.PlayerNameModule.Services;

namespace Modules.LobbyModule.Controllers
{
    /// <summary>A lobby is entered under a name, so a blank one ends the flow before any network call.</summary>
    internal class SavePlayerNameCommand : Command
    {
        [Inject] private ILobbyModel _model { get; set; }
        [Inject] private IPlayerNameService _playerNames { get; set; }
        [InjectSignal] private LobbySignals _signals { get; set; }

        // Create carries the name alone, Join carries it before the code: the first string either way.
        [SignalParam(0)] private string _playerName { get; set; }

        public override void Execute()
        {
            if (string.IsNullOrWhiteSpace(_playerName))
            {
                Retain();
                this.StopWith(_model, _signals, LobbyConstants.STATUS_NAME_MISSING);
                return;
            }

            _playerNames.Set(_playerName);
        }
    }
}

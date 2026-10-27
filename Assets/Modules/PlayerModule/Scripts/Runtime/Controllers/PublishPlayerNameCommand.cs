using FlowIoC.BaseModule.Controller;
using FlowIoC.BaseModule.Injectable.Attributes;
using Modules.PlayerModule.Entities;
using Modules.PlayerModule.Models;
using Modules.PlayerModule.Signals;

namespace Modules.PlayerModule.Controllers
{
    /// <summary>The scoreboard is kept by the server, so only the server announces a name.</summary>
    internal class PublishPlayerNameCommand : Command
    {
        [Inject] private IPlayerModel _model { get; set; }
        [InjectSignal] private PlayerSignals _signals { get; set; }

        [SignalParam] private ulong _clientId { get; set; }
        [SignalParam] private string _playerName { get; set; }

        public override void Execute()
        {
            if (_model.TryGet(_clientId, out IPlayerAvatar avatar) && avatar.IsServer)
                _signals.Outgoing.PlayerNameChanged.Dispatch(_clientId, _playerName);
        }
    }
}

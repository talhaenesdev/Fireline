using FlowIoC.BaseModule.Controller;
using FlowIoC.BaseModule.Injectable.Attributes;
using Modules.PlayerModule.Entities;
using Modules.PlayerModule.Models;
using Modules.PlayerNameModule.Services;

namespace Modules.PlayerModule.Controllers
{
    /// <summary>The server cleans what a client asked to be called and puts it on that client's avatar.</summary>
    internal class ApplyPlayerNameCommand : Command
    {
        [Inject] private IPlayerModel _model { get; set; }
        [Inject] private IPlayerNameService _playerNames { get; set; }

        [SignalParam] private ulong _clientId { get; set; }
        [SignalParam] private string _playerName { get; set; }

        public override void Execute()
        {
            if (_model.TryGet(_clientId, out IPlayerAvatar avatar) && avatar.IsServer)
                avatar.SetNameServer(_playerNames.Clean(_playerName));
        }
    }
}

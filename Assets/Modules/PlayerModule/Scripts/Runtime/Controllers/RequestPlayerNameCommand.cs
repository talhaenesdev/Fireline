using FlowIoC.BaseModule.Controller;
using FlowIoC.BaseModule.Injectable.Attributes;
using Modules.PlayerModule.Entities;
using Modules.PlayerNameModule.Services;

namespace Modules.PlayerModule.Controllers
{
    /// <summary>The local avatar carries the name typed on the menu; the server decides what it becomes.</summary>
    internal class RequestPlayerNameCommand : Command
    {
        [Inject] private IPlayerNameService _playerNames { get; set; }

        [SignalParam] private IPlayerAvatar _avatar { get; set; }

        public override void Execute() => _avatar.RequestName(_playerNames.Get());
    }
}

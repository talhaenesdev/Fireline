using FlowIoC.BaseModule.Controller;
using FlowIoC.BaseModule.Injectable.Attributes;
using Modules.PlayerModule.Entities;
using Modules.PlayerModule.Models;

namespace Modules.PlayerModule.Controllers
{
    internal class RegisterAvatarCommand : Command
    {
        [Inject] private IPlayerModel _model { get; set; }

        [SignalParam] private IPlayerAvatar _avatar { get; set; }

        public override void Execute() => _model.Add(_avatar);
    }
}

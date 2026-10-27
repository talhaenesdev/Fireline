using FlowIoC.BaseModule.Controller;
using FlowIoC.BaseModule.Injectable.Attributes;
using Modules.PlayerModule.Models;

namespace Modules.PlayerModule.Controllers
{
    internal class StoreFriendlyFireCommand : Command
    {
        [Inject] private IPlayerModel _model { get; set; }

        [SignalParam] private bool _isFriendlyFire { get; set; }

        public override void Execute() => _model.IsFriendlyFire = _isFriendlyFire;
    }
}

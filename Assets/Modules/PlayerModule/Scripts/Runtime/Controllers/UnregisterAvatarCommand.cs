using FlowIoC.BaseModule.Controller;
using FlowIoC.BaseModule.Injectable.Attributes;
using Modules.PlayerModule.Entities;
using Modules.PlayerModule.Models;
using Modules.PlayerModule.Signals;

namespace Modules.PlayerModule.Controllers
{
    /// <summary>A despawned avatar leaves the registry; losing the local one disarms the player until respawn.</summary>
    internal class UnregisterAvatarCommand : Command
    {
        [Inject] private IPlayerModel _model { get; set; }
        [InjectSignal] private PlayerSignals _signals { get; set; }

        [SignalParam] private IPlayerAvatar _avatar { get; set; }

        public override void Execute()
        {
            bool wasLocal = _model.LocalAvatar == _avatar;
            _model.Remove(_avatar);

            if (!wasLocal)
                return;

            _model.IsLocalAlive = false;
            _signals.Outgoing.LocalAliveChanged.Dispatch(false);
        }
    }
}

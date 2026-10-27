using FlowIoC.BaseModule.Controller;
using FlowIoC.BaseModule.Injectable.Attributes;
using Modules.PlayerModule.Models;
using Modules.PlayerModule.Signals;

namespace Modules.PlayerModule.Controllers
{
    /// <summary>A dead local player stops moving and firing until the server brings it back.</summary>
    internal class ApplyLocalDeathCommand : Command
    {
        [Inject] private IPlayerModel _model { get; set; }
        [InjectSignal] private PlayerSignals _signals { get; set; }

        [SignalParam] private ulong _clientId { get; set; }
        [SignalParam] private bool _isDead { get; set; }

        public override void Execute()
        {
            if (_model.LocalAvatar == null || _model.LocalAvatar.ClientId != _clientId)
                return;

            _model.IsLocalAlive = !_isDead;
            _signals.Outgoing.LocalAliveChanged.Dispatch(!_isDead);
        }
    }
}

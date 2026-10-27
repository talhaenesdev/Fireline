using FlowIoC.BaseModule.Controller;
using FlowIoC.BaseModule.Injectable.Attributes;
using Modules.PlayerModule.Models;
using Modules.PlayerModule.Signals;

namespace Modules.PlayerModule.Controllers
{
    /// <summary>The HUD shows the local player's health and nobody else's.</summary>
    internal class PublishLocalHealthCommand : Command
    {
        [Inject] private IPlayerModel _model { get; set; }
        [InjectSignal] private PlayerSignals _signals { get; set; }

        [SignalParam] private ulong _clientId { get; set; }
        [SignalParam(0)] private float _health { get; set; }
        [SignalParam(1)] private float _maxHealth { get; set; }

        public override void Execute()
        {
            if (_model.LocalAvatar != null && _model.LocalAvatar.ClientId == _clientId)
                _signals.Outgoing.LocalHealthChanged.Dispatch(_health, _maxHealth);
        }
    }
}

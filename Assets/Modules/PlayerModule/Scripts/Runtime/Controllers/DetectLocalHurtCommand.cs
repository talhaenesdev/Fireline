using FlowIoC.BaseModule.Controller;
using FlowIoC.BaseModule.Injectable.Attributes;
using Modules.PlayerModule.Models;
using Modules.PlayerModule.Signals;

namespace Modules.PlayerModule.Controllers
{
    /// <summary>The local player's health went down: the HUD flashes and the camera shakes.</summary>
    internal class DetectLocalHurtCommand : Command
    {
        [Inject] private IPlayerModel _model { get; set; }
        [InjectSignal] private PlayerSignals _signals { get; set; }
        [InjectSignal] private PlayerInternalSignals _internalSignals { get; set; }

        [SignalParam] private ulong _clientId { get; set; }
        [SignalParam(0)] private float _health { get; set; }

        public override void Execute()
        {
            if (_model.LocalAvatar == null || _model.LocalAvatar.ClientId != _clientId)
                return;

            float previous = _model.LastLocalHealth;
            _model.LastLocalHealth = _health;

            if (previous < 0f || _health >= previous)
                return;

            _signals.Outgoing.LocalHurt.Dispatch(previous - _health);
            _internalSignals.ShakeLocalCamera.Dispatch();
        }
    }
}

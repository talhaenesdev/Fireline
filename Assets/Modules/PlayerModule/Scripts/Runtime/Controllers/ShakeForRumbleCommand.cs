using FlowIoC.BaseModule.Controller;
using FlowIoC.BaseModule.Injectable.Attributes;
using Modules.PlayerModule.Models;
using Modules.PlayerModule.Signals;
using UnityEngine;

namespace Modules.PlayerModule.Controllers
{
    /// <summary>A blast or a fall near the local player shakes their camera, harder the closer it was.</summary>
    internal class ShakeForRumbleCommand : Command
    {
        private const float REACH = 20f;
        private const float STRONGEST = 10f;

        [Inject] private IPlayerModel _model { get; set; }
        [InjectSignal] private PlayerInternalSignals _internalSignals { get; set; }

        [SignalParam] private Vector3 _position { get; set; }

        public override void Execute()
        {
            if (_model.LocalAvatar == null || _model.LocalAvatar.Transform == null)
                return;

            Vector3 offset = _model.LocalAvatar.Transform.position - _position;
            offset.y = 0f;
            float distance = offset.magnitude;
            if (distance >= REACH)
                return;

            _internalSignals.ShakeLocalCameraBy.Dispatch(Mathf.Lerp(STRONGEST, 1f, distance / REACH));
        }
    }
}

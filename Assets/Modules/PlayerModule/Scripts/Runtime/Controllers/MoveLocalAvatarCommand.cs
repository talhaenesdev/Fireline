using FlowIoC.BaseModule.Controller;
using FlowIoC.BaseModule.Injectable.Attributes;
using Modules.PlayerModule.Entities;
using Modules.PlayerModule.Models;
using Modules.PlayerModule.Signals;
using UnityEngine;

namespace Modules.PlayerModule.Controllers
{
    /// <summary>One frame of the local player's movement: how far it goes, at walking or sprinting speed.</summary>
    internal class MoveLocalAvatarCommand : Command
    {
        [Inject] private IPlayerModel _model { get; set; }
        [InjectSignal] private PlayerInternalSignals _internalSignals { get; set; }

        public override void Execute()
        {
            IPlayerAvatar avatar = _model.LocalAvatar;
            if (avatar == null)
                return;

            // A dead player stands still and its legs stop.
            Vector2 input = _model.IsLocalAlive ? _model.MoveInput : Vector2.zero;
            bool isSprinting = _model.IsLocalAlive && _model.IsSprinting;
            float speed = (isSprinting ? avatar.SprintSpeed : avatar.MoveSpeed) * avatar.SpeedScale;

            Vector3 step = new Vector3(input.x, 0f, input.y) * (speed * Time.deltaTime);
            _internalSignals.MoveLocalAvatar.Dispatch(step, input, isSprinting);
        }
    }
}

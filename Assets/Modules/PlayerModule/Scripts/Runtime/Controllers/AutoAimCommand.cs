using FlowIoC.BaseModule.Controller;
using FlowIoC.BaseModule.Injectable.Attributes;
using Modules.PlayerModule.Entities;
using Modules.PlayerModule.Models;
using Modules.PlayerModule.Signals;
using UnityEngine;

namespace Modules.PlayerModule.Controllers
{
    /// <summary>
    /// On a touchscreen the avatar aims by itself: at the nearest zombie in range, or the way it is
    /// walking when none is near. The weapon fires where the avatar faces.
    /// </summary>
    internal class AutoAimCommand : Command
    {
        private const string ZOMBIE_TAG = "Zombie";
        private const float AUTO_AIM_RANGE = 25f;
        private const float WALK_AIM_DISTANCE = 10f;

        [Inject] private IPlayerModel _model { get; set; }
        [InjectSignal] private PlayerSignals _signals { get; set; }
        [InjectSignal] private PlayerInternalSignals _internalSignals { get; set; }

        public override void Execute()
        {
            IPlayerAvatar avatar = _model.LocalAvatar;
            if (!_model.IsTouchInput || avatar == null || !_model.IsLocalAlive)
                return;

            Vector3 origin = avatar.Transform.position;
            Vector3 direction = Vector3.zero;
            Vector3 target = Vector3.zero;
            float nearest = AUTO_AIM_RANGE * AUTO_AIM_RANGE;

            foreach (GameObject zombie in GameObject.FindGameObjectsWithTag(ZOMBIE_TAG))
            {
                Vector3 offset = zombie.transform.position - origin;
                offset.y = 0f;

                if (offset.sqrMagnitude >= nearest)
                    continue;

                nearest = offset.sqrMagnitude;
                direction = offset;
                target = zombie.transform.position;
            }

            if (direction == Vector3.zero)
            {
                direction = new Vector3(_model.MoveInput.x, 0f, _model.MoveInput.y);
                target = origin + direction.normalized * WALK_AIM_DISTANCE;
            }

            if (direction.sqrMagnitude < 0.001f)
                return;

            // The avatar turns to the zombie, and the round is sent at the zombie itself from the muzzle.
            _internalSignals.FaceLocalAvatar.Dispatch(direction.normalized);
            _model.AimPoint = target;
            _signals.Outgoing.AimChanged.Dispatch(target);
        }
    }
}

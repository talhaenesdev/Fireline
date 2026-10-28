using FlowIoC.BaseModule.Controller;
using FlowIoC.BaseModule.Injectable.Attributes;
using Modules.WeaponModule.Data.UnityObjects;
using Modules.WeaponModule.Entities;
using Modules.WeaponModule.Models;
using UnityEngine;

namespace Modules.WeaponModule.Controllers
{
    /// <summary>
    /// Decides whether the local weapon fires this frame. An automatic weapon fires while the
    /// trigger is held, a semi-automatic one once per press, and neither while reloading, empty or
    /// faster than its rate. A frame that does not fire ends the tick's run here.
    /// </summary>
    internal class FireWeaponCommand : Command
    {
        [Inject] private IWeaponModel _model { get; set; }

        public override void Execute()
        {
            bool wasPressed = _model.WasTriggerPressed;
            _model.WasTriggerPressed = false;

            IWeapon weapon = _model.LocalWeapon;
            CD_Weapon config = weapon?.Config;

            bool wantsToFire = config != null && (config.Automatic ? _model.IsTriggerHeld : wasPressed);
            bool canFire = wantsToFire
                           && _model.IsArmed
                           && !_model.IsReloading
                           && _model.Ammo > 0
                           && Time.time >= _model.NextFireTime
                           && _model.HasAim;

            if (!canFire)
            {
                Retain();
                Stop();
                return;
            }

            // From the muzzle to the aimed point, level with the ground, so the round meets what was aimed at.
            Vector3 muzzle = weapon.MuzzlePosition;
            Vector3 direction = _model.AimPoint - muzzle;
            direction.y = 0f;

            if (direction.sqrMagnitude < 0.01f)
            {
                Retain();
                Stop();
                return;
            }

            _model.Ammo--;
            _model.NextFireTime = Time.time + config.FireRate * _model.FireRateMultiplier;
            _model.ShotPosition = muzzle;
            _model.ShotDirection = direction.normalized;
        }
    }
}

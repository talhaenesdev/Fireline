using FlowIoC.BaseModule.Controller;
using FlowIoC.BaseModule.Injectable.Attributes;
using Modules.WeaponModule.Entities;
using Modules.WeaponModule.Models;

namespace Modules.WeaponModule.Controllers
{
    /// <summary>
    /// A spawned weapon that belongs to this process becomes the local weapon, with a full
    /// magazine. Anybody else's weapon ends the flow here: the server reaches it through its RPCs.
    /// </summary>
    internal class RegisterLocalWeaponCommand : Command
    {
        [Inject] private IWeaponModel _model { get; set; }

        [SignalParam] private IWeapon _weapon { get; set; }

        public override void Execute()
        {
            if (!_weapon.IsOwner || _weapon.Config == null)
            {
                Retain();
                Stop();
                return;
            }

            _model.LocalWeapon = _weapon;
            _model.Ammo = _weapon.Config.MagazineSize;
            _model.IsReloading = false;
            _model.NextFireTime = 0f;
            _model.IsTriggerHeld = false;
            _model.WasTriggerPressed = false;
        }
    }
}

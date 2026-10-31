using FlowIoC.BaseModule.Controller;
using FlowIoC.BaseModule.Injectable.Attributes;
using FlowIoC.BaseModule.Provider.Coroutine;
using Modules.WeaponModule.Models;

namespace Modules.WeaponModule.Controllers
{
    /// <summary>A reload starts when there is a weapon, it is not already reloading and the magazine is not full.</summary>
    internal class StartReloadCommand : Command
    {
        [Inject] private IWeaponModel _model { get; set; }

        public override void Execute()
        {
            bool canReload = _model.LocalWeapon?.Config != null
                             && _model.IsArmed
                             && !_model.IsReloading
                             && _model.Ammo < _model.LocalWeapon.Config.MagazineSize;

            if (canReload)
            {
                _model.IsReloading = true;
                return;
            }

            Retain();
            Stop();
        }
    }

    internal class WaitReloadCommand : Command
    {
        [Inject] private IWeaponModel _model { get; set; }
        [Inject] private ICoroutineProvider _coroutineProvider { get; set; }

        public override void Execute()
        {
            Retain();
            _coroutineProvider.WaitForSeconds(_model.LocalWeapon.Config.ReloadDuration * _model.ReloadMultiplier, () => Release());
        }
    }

    internal class CompleteReloadCommand : Command
    {
        [Inject] private IWeaponModel _model { get; set; }

        public override void Execute()
        {
            _model.IsReloading = false;

            // The player may have died and respawned mid-reload; the new weapon is already full.
            if (_model.LocalWeapon?.Config != null)
                _model.Ammo = _model.LocalWeapon.Config.MagazineSize;
        }
    }
}

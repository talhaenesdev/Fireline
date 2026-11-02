using FlowIoC.BaseModule.Controller;
using FlowIoC.BaseModule.Injectable.Attributes;
using FlowIoC.BaseModule.Provider.Coroutine;
using Modules.WeaponModule.Models;

namespace Modules.WeaponModule.Controllers
{
    /// <summary>A reload starts when there is a weapon, it is not already reloading, the magazine is not full and there are rounds to load.</summary>
    internal class StartReloadCommand : Command
    {
        [Inject] private IWeaponModel _model { get; set; }

        public override void Execute()
        {
            bool canReload = _model.LocalWeapon?.Config != null
                             && _model.IsArmed
                             && !_model.IsReloading
                             && _model.Ammo < _model.LocalWeapon.Config.MagazineSize
                             && (_model.LocalWeapon.Config.InfiniteReserve || _model.Reserve > 0);

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
            var config = _model.LocalWeapon?.Config;
            if (config == null)
                return;

            // The magazine takes what it lacks from the spare rounds, as far as they go.
            int wanted = config.MagazineSize - _model.Ammo;
            int loaded = config.InfiniteReserve ? wanted : UnityEngine.Mathf.Min(wanted, _model.Reserve);
            _model.Ammo += loaded;
            if (!config.InfiniteReserve)
                _model.Reserve -= loaded;
        }
    }
}

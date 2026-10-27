using FlowIoC.BaseModule.Controller;
using FlowIoC.BaseModule.Injectable.Attributes;
using Modules.WeaponModule.Models;
using Modules.WeaponModule.Signals;

namespace Modules.WeaponModule.Controllers
{
    /// <summary>An empty magazine reloads by itself.</summary>
    internal class AutoReloadCommand : Command
    {
        [Inject] private IWeaponModel _model { get; set; }
        [InjectSignal] private WeaponInternalSignals _internalSignals { get; set; }

        public override void Execute()
        {
            if (_model.LocalWeapon != null && _model.IsArmed && _model.Ammo <= 0 && !_model.IsReloading)
                _internalSignals.Reload.Dispatch();
        }
    }
}

using FlowIoC.BaseModule.Controller;
using FlowIoC.BaseModule.Injectable.Attributes;
using Modules.WeaponModule.Models;
using Modules.WeaponModule.Signals;

namespace Modules.WeaponModule.Controllers
{
    internal class PublishAmmoCommand : Command
    {
        [Inject] private IWeaponModel _model { get; set; }
        [InjectSignal] private WeaponSignals _signals { get; set; }

        public override void Execute()
        {
            int magazineSize = _model.LocalWeapon?.Config != null ? _model.LocalWeapon.Config.MagazineSize : 0;
            _signals.Outgoing.AmmoChanged.Dispatch(_model.Ammo, magazineSize);
        }
    }
}

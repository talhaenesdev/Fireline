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
            var config = _model.LocalWeapon?.Config;
            int magazineSize = config != null ? config.MagazineSize : 0;
            int reserve = config != null && config.InfiniteReserve ? -1 : _model.Reserve;
            _signals.Outgoing.AmmoChanged.Dispatch(_model.Ammo, magazineSize, reserve);
        }
    }
}

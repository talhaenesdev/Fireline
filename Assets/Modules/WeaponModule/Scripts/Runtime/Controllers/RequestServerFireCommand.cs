using FlowIoC.BaseModule.Controller;
using FlowIoC.BaseModule.Injectable.Attributes;
using Modules.WeaponModule.Models;

namespace Modules.WeaponModule.Controllers
{
    internal class RequestServerFireCommand : Command
    {
        [Inject] private IWeaponModel _model { get; set; }

        public override void Execute() => _model.LocalWeapon.RequestServerFire(_model.ShotPosition, _model.ShotDirection);
    }
}

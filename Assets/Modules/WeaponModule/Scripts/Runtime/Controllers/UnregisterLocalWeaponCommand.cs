using FlowIoC.BaseModule.Controller;
using FlowIoC.BaseModule.Injectable.Attributes;
using Modules.WeaponModule.Entities;
using Modules.WeaponModule.Models;

namespace Modules.WeaponModule.Controllers
{
    internal class UnregisterLocalWeaponCommand : Command
    {
        [Inject] private IWeaponModel _model { get; set; }

        [SignalParam] private IWeapon _weapon { get; set; }

        public override void Execute()
        {
            if (_model.LocalWeapon == _weapon)
                _model.LocalWeapon = null;
        }
    }
}

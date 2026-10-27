using FlowIoC.BaseModule.Controller;
using FlowIoC.BaseModule.Injectable.Attributes;
using Modules.PlayerModule.HudScreenModule.Models;

namespace Modules.PlayerModule.HudScreenModule.Controllers
{
    internal class RememberHealthCommand : Command
    {
        [Inject] private IHudScreenModel _model { get; set; }

        [SignalParam(0)] private float _health { get; set; }
        [SignalParam(1)] private float _maxHealth { get; set; }

        public override void Execute()
        {
            _model.Health = _health;
            _model.MaxHealth = _maxHealth;
        }
    }

    internal class RememberAmmoCommand : Command
    {
        [Inject] private IHudScreenModel _model { get; set; }

        [SignalParam(0)] private int _ammo { get; set; }
        [SignalParam(1)] private int _magazineSize { get; set; }

        public override void Execute()
        {
            _model.Ammo = _ammo;
            _model.MagazineSize = _magazineSize;
        }
    }
}

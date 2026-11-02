using FlowIoC.BaseModule.Controller;
using FlowIoC.BaseModule.Injectable.Attributes;
using Modules.WeaponModule.Models;
using UnityEngine;

namespace Modules.WeaponModule.Controllers
{
    /// <summary>Spare rounds bought for the local weapon, counted in its magazines, up to what it can carry.</summary>
    internal class RefillAmmoCommand : Command
    {
        [Inject] private IWeaponModel _model { get; set; }

        [SignalParam] private float _magazines { get; set; }
        [SignalParam] private int _weaponIndex { get; set; }

        public override void Execute()
        {
            var config = _model.LocalWeapon?.Config;
            if (config == null || config.InfiniteReserve || (_weaponIndex >= 0 && _model.LocalWeapon.WeaponIndex != _weaponIndex))
                return;

            int rounds = Mathf.CeilToInt(_magazines * config.MagazineSize);
            _model.Reserve = Mathf.Min(config.MaxReserve, _model.Reserve + rounds);
        }
    }
}

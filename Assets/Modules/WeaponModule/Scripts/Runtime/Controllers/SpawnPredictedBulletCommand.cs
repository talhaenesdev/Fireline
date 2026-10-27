using FlowIoC.BaseModule.Controller;
using FlowIoC.BaseModule.Injectable.Attributes;
using FlowIoC.PoolModule.Services;
using Modules.WeaponModule.Data.UnityObjects;
using Modules.WeaponModule.Entities;
using Modules.WeaponModule.Models;

namespace Modules.WeaponModule.Controllers
{
    /// <summary>The shooter sees its round leave at once rather than one round trip later.</summary>
    internal class SpawnPredictedBulletCommand : Command
    {
        [Inject] private IWeaponModel _model { get; set; }
        [Inject] private IPoolService _poolService { get; set; }

        public override void Execute()
        {
            CD_Weapon config = _model.LocalWeapon.Config;

            PredictedBullet bullet = _poolService.Get<PredictedBullet>(config.PredictedBulletPoolKey);
            if (bullet == null)
                return;

            bullet.Launch(_model.ShotPosition, _model.ShotDirection, config.BulletSpeed, config.BulletLifetime,
                _model.LocalWeapon.Owner);
        }
    }
}

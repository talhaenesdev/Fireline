using FlowIoC.BaseModule.Controller;
using FlowIoC.BaseModule.Injectable.Attributes;
using FlowIoC.PoolModule.Services;
using Modules.WeaponModule.Entities;
using Modules.WeaponModule.Models;
using UnityEngine;

namespace Modules.WeaponModule.Controllers
{
    internal class PlayImpactEffectCommand : Command
    {
        // The key every weapon's config names; a client sees impacts before it has a weapon of its own.
        private const string DEFAULT_IMPACT_POOL_KEY = "WallImpact";

        [Inject] private IWeaponModel _model { get; set; }
        [Inject] private IPoolService _poolService { get; set; }

        [SignalParam] private Vector3 _point { get; set; }

        public override void Execute()
        {
            string key = _model.LocalWeapon?.Config != null ? _model.LocalWeapon.Config.ImpactPoolKey : DEFAULT_IMPACT_POOL_KEY;

            PooledParticleEffect effect = _poolService.Get<PooledParticleEffect>(key);
            if (effect != null)
                effect.PlayAt(_point);
        }
    }
}

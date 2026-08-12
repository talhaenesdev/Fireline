using UnityEngine;
using FireLine.Scripts.Pooling;
using FireLine.Scripts.Weapon.Model;
using FireLine.Scripts.Weapon.View;

namespace FireLine.Scripts.Weapon.Controller
{
    public class WeaponController
    {
        private readonly IPoolService _poolService;
        private readonly WeaponData _weaponData;

        public WeaponController(
            IPoolService poolService,
            WeaponData weaponData)
        {
            _poolService = poolService;
            _weaponData = weaponData;
        }

        public void Shoot(
            Vector3 position,
            Vector3 direction)
        {
            BulletData bulletData =
                _weaponData.BulletData;

            if (bulletData == null)
            {
                Debug.LogError(
                    "Weapon has no BulletData."
                );

                return;
            }

            BulletView bullet =
                _poolService.Spawn<BulletView>(
                    bulletData.PoolKey,
                    position,
                    Quaternion.LookRotation(direction)
                );

            if (bullet == null)
                return;

            bullet.Initialize(
                bulletData,
                direction,
                _poolService
            );
        }
    }
}
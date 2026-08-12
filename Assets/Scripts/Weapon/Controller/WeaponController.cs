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

        private float _nextFireTime;

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
            if (Time.time < _nextFireTime)
                return;

            if (_weaponData == null)
            {
                Debug.LogError("WeaponData is null.");
                return;
            }

            if (_weaponData.BulletData == null)
            {
                Debug.LogError("BulletData is null.");
                return;
            }

            BulletData bulletData =
                _weaponData.BulletData;

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

            _nextFireTime =
                Time.time + _weaponData.FireRate;
        }
    }
}
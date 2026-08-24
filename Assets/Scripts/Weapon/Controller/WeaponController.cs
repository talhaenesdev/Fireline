using FireLine.Scripts.Weapon.Model;
using UnityEngine;

namespace FireLine.Scripts.Weapon.Controller
{
    public class WeaponController
    {
        private readonly WeaponData _weaponData;

        private float _nextFireTime;

        public WeaponController(
            WeaponData weaponData)
        {
            _weaponData = weaponData;
        }

        public bool CanShoot()
        {
            if (_weaponData == null)
            {
                Debug.LogError(
                    "[WEAPON] WeaponData is NULL!"
                );

                return false;
            }

            return Time.time >= _nextFireTime;
        }

        public void RegisterShot()
        {
            if (_weaponData == null)
                return;

            _nextFireTime =
                Time.time +
                _weaponData.FireRate;
        }
    }
}
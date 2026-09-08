using FireLine.Scripts.Weapon.Model;
using UnityEngine;

namespace FireLine.Scripts.Weapon.Controller
{
    public class WeaponController
    {
        private readonly WeaponData _weaponData;

        private float _nextFireTime;
        private int _currentAmmo;
        private bool _isReloading;

        public float ReloadDuration =>
            _weaponData != null
            ? _weaponData.ReloadDuration
            : 0f;

        public WeaponController(
            WeaponData weaponData)
        {
            _weaponData = weaponData;

            if (_weaponData != null)
            {
                _currentAmmo =
                    _weaponData.MagazineSize;
            }
        }

        public int CurrentAmmo =>
            _currentAmmo;

        public int MagazineSize =>
            _weaponData != null
                ? _weaponData.MagazineSize
                : 0;

        public bool IsReloading =>
            _isReloading;

        public bool IsAutomatic()
        {
            return _weaponData != null &&
                   _weaponData.Automatic;
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

            if (_isReloading)
            {
                Debug.Log(
                    "[WEAPON] Shoot blocked | Reloading"
                );

                return false;
            }

            if (_currentAmmo <= 0)
            {
                Debug.Log(
                    "[WEAPON] Shoot blocked | Magazine empty"
                );

                return false;
            }

            return Time.time >= _nextFireTime;
        }

        public void RegisterShot()
        {
            if (_weaponData == null)
                return;

            if (_currentAmmo <= 0)
                return;

            _currentAmmo--;

            _nextFireTime =
                Time.time +
                _weaponData.FireRate;

            Debug.Log(
                $"[WEAPON] Shot registered | " +
                $"Ammo={_currentAmmo}/" +
                $"{_weaponData.MagazineSize}"
            );
        }

        public bool CanReload()
        {
            if (_weaponData == null)
                return false;

            if (_isReloading)
                return false;

            if (_currentAmmo >=
                _weaponData.MagazineSize)
            {
                return false;
            }

            return true;
        }

        public bool StartReload()
        {
            if (!CanReload())
                return false;

            _isReloading = true;

            Debug.Log(
                $"[WEAPON] Reload started | " +
                $"Ammo={_currentAmmo}/" +
                $"{_weaponData.MagazineSize} | " +
                $"Duration={_weaponData.ReloadDuration}"
            );

            return true;
        }

        public void CompleteReload()
        {
            if (_weaponData == null)
                return;

            _currentAmmo =
                _weaponData.MagazineSize;

            _isReloading = false;

            Debug.Log(
                $"[WEAPON] Reload completed | " +
                $"Ammo={_currentAmmo}/" +
                $"{_weaponData.MagazineSize}"
            );
        }
    }
}
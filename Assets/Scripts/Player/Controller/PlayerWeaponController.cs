using FireLine.Scripts.Core.Weapon;
using FireLine.Scripts.Weapon.Controller;
using FireLine.Scripts.Weapon.Service;
using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace FireLine.Scripts.Player.Controller
{
    public class PlayerWeaponController : MonoBehaviour
    {
        private WeaponController _weaponController;

        private IWeaponFireService _fireService;

        private NetworkObject _networkObject;

        [SerializeField]
        private Transform muzzlePoint;
        public event System.Action<int, int> AmmoChanged;
        [Inject]
        public void Construct(
            WeaponController weaponController)
        {
            _weaponController =
                weaponController;

            Debug.Log(
                $"[PLAYER-WEAPON][INJECT] " +
                $"WeaponController injected | " +
                $"Instance={GetInstanceID()} | " +
                $"Scene={gameObject.scene.name}"
            );
        }

        private void Awake()
        {
            _networkObject =
                GetComponent<NetworkObject>();

            _fireService =
                GetComponent<IWeaponFireService>();

            Debug.Log(
                $"[PLAYER-WEAPON][AWAKE] " +
                $"Instance={GetInstanceID()} | " +
                $"GameObject={gameObject.name} | " +
                $"Scene={gameObject.scene.name} | " +
                $"NetworkObject={_networkObject != null} | " +
                $"FireService={_fireService != null}"
            );
        }

        public int CurrentAmmo =>
            _weaponController != null
                ? _weaponController.CurrentAmmo
                : 0;

        public int MagazineSize =>
            _weaponController != null
                ? _weaponController.MagazineSize
                : 0;

        public bool IsReloading =>
            _weaponController != null &&
            _weaponController.IsReloading;

        public float ReloadDuration =>
            _weaponController != null
                ? _weaponController.ReloadDuration
                : 0f;

        public bool IsAutomatic()
        {
            return _weaponController != null &&
                   _weaponController.IsAutomatic();
        }

        public bool StartReload()
        {
            if (_weaponController == null)
            {
                Debug.LogError(
                    "[PLAYER-WEAPON] " +
                    "WeaponController is NULL!"
                );

                return false;
            }

            bool started =
                _weaponController.StartReload();

            if (!started)
            {
                Debug.Log(
                    "[PLAYER-WEAPON] " +
                    "Reload could not start."
                );

                return false;
            }

            Debug.Log(
                "[PLAYER-WEAPON] " +
                "Reload started."
            );

            PlayReloadSound();

            return true;
        }

        public void CompleteReload()
        {
            if (_weaponController == null)
            {
                Debug.LogError(
                    "[PLAYER-WEAPON] " +
                    "WeaponController is NULL!"
                );

                return;
            }

            _weaponController.CompleteReload();

            AmmoChanged?.Invoke(
                _weaponController.CurrentAmmo,
                _weaponController.MagazineSize
            );
        }

        private void PlayReloadSound()
        {
            if (WeaponAudioManager.Instance == null)
            {
                Debug.LogWarning(
                    "[PLAYER-WEAPON] " +
                    "WeaponAudioManager is NULL!"
                );

                return;
            }

            if (muzzlePoint == null)
            {
                Debug.LogWarning(
                    "[PLAYER-WEAPON] " +
                    "MuzzlePoint is NULL!"
                );

                return;
            }

            WeaponAudioManager.Instance.PlayReloadSound(
                muzzlePoint.position
            );
        }

        public void Shoot(Vector3 direction)
        {
            Debug.Log(
                $"[PLAYER-WEAPON][SHOOT] " +
                $"Instance={GetInstanceID()} | " +
                $"GameObject={gameObject.name} | " +
                $"Owner={GetOwnerId()} | " +
                $"IsOwner={GetIsOwner()} | " +
                $"Controller={_weaponController != null}"
            );

            if (_weaponController == null)
            {
                Debug.LogError(
                    "[PLAYER-WEAPON][ERROR] " +
                    "WeaponController is NULL!"
                );

                return;
            }

            if (_fireService == null)
            {
                Debug.LogError(
                    "[PLAYER-WEAPON][ERROR] " +
                    "IWeaponFireService is NULL!"
                );

                return;
            }

            if (muzzlePoint == null)
            {
                Debug.LogError(
                    "[PLAYER-WEAPON][ERROR] " +
                    "MuzzlePoint is NULL!"
                );

                return;
            }

            if (direction == Vector3.zero)
            {
                Debug.LogWarning(
                    "[PLAYER-WEAPON][SHOOT] " +
                    "Shoot cancelled | Direction is zero"
                );

                return;
            }

            if (!_weaponController.CanShoot())
            {
                Debug.Log(
                    "[PLAYER-WEAPON][SHOOT] " +
                    "Shoot blocked | WeaponController.CanShoot=false"
                );

                return;
            }

            Vector3 position =
                muzzlePoint.position;

            _weaponController.RegisterShot();

            Debug.Log(
                $"[PLAYER-WEAPON][FIRE] " +
                $"Position={position} | " +
                $"Direction={direction}"
            );

            AmmoChanged?.Invoke(
                _weaponController.CurrentAmmo,
                _weaponController.MagazineSize
            );

            _fireService.Fire(
                position,
                direction
            );
        }

        private ulong GetOwnerId()
        {
            return _networkObject != null
                ? _networkObject.OwnerClientId
                : ulong.MaxValue;
        }

        private bool GetIsOwner()
        {
            return _networkObject != null &&
                   _networkObject.IsOwner;
        }
    }
}
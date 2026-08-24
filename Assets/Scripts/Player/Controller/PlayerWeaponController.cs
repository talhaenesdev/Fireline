using FireLine.Scripts.Core.Weapon;
using FireLine.Scripts.Weapon.Controller;
using UnityEngine;
using Zenject;

namespace FireLine.Scripts.Player.Controller
{
    public class PlayerWeaponController : MonoBehaviour
    {
        private WeaponController _weaponController;
        private IWeaponFireService _fireService;

        [SerializeField]
        private Transform muzzlePoint;

        [Inject]
        public void Construct(
            WeaponController weaponController)
        {
            _weaponController =
                weaponController;

            Debug.Log(
                "[PLAYER WEAPON] " +
                "WeaponController injected successfully."
            );
        }

        private void Awake()
        {
            _fireService =
                GetComponent<
                    IWeaponFireService>();

            if (_fireService == null)
            {
                Debug.LogError(
                    "[PLAYER WEAPON] " +
                    "IWeaponFireService NOT FOUND!"
                );
            }
        }

        public void Shoot(
            Vector3 direction)
        {
            if (_weaponController == null)
            {
                Debug.LogError(
                    "[PLAYER WEAPON] " +
                    "WeaponController is NULL!"
                );

                return;
            }

            if (_fireService == null)
            {
                Debug.LogError(
                    "[PLAYER WEAPON] " +
                    "IWeaponFireService is NULL!"
                );

                return;
            }

            if (muzzlePoint == null)
            {
                Debug.LogError(
                    "[PLAYER WEAPON] " +
                    "MuzzlePoint is NULL!"
                );

                return;
            }

            if (direction == Vector3.zero)
                return;

            if (!_weaponController.CanShoot())
                return;

            Vector3 position =
                muzzlePoint.position;

            _weaponController.RegisterShot();

            Debug.Log(
                $"[PLAYER WEAPON] Shoot | " +
                $"Position: {position} | " +
                $"Direction: {direction}"
            );

            _fireService.Fire(
                position,
                direction
            );
        }
    }
}
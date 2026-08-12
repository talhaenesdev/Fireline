using UnityEngine;
using Zenject;
using FireLine.Scripts.Weapon.Controller;
using FireLine.Scripts.Weapon.View;

namespace FireLine.Scripts.Player.Controller
{
    public class PlayerWeaponController : MonoBehaviour
    {
        [SerializeField]
        private WeaponView weaponView;

        private WeaponController _weaponController;

        [Inject]
        public void Construct(
            WeaponController weaponController)
        {
            _weaponController =
                weaponController;
        }

        public void Shoot(Vector3 direction)
        {
            Debug.Log($"WEAPON SHOOT: {direction}");

            if (_weaponController == null)
            {
                Debug.LogError("WeaponController is NULL!");
                return;
            }

            if (weaponView == null)
            {
                Debug.LogError("WeaponView is NULL!");
                return;
            }

            if (weaponView.MuzzlePoint == null)
            {
                Debug.LogError("MuzzlePoint is NULL!");
                return;
            }

            _weaponController.Shoot(
                weaponView.MuzzlePoint.position,
                direction
            );
        }
    }
}
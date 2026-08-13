using FireLine.Scripts.Weapon.Controller;
using FireLine.Scripts.Weapon.View;
using UnityEngine;
using Zenject;

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
            _weaponController = weaponController;
        }

        public void Shoot(Vector3 direction)
        {
            if (_weaponController == null)
                return;


            if (weaponView == null)
                return;

            if (weaponView.MuzzlePoint == null)
                return;

            _weaponController.Shoot(
                weaponView.MuzzlePoint.position,
                direction.normalized
            );
        }
    }
}
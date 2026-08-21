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
            _weaponController = weaponController;
            Debug.Log("WeaponController injected successfully.");
        }

        public void Shoot(Vector3 direction)
        {

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
        public Vector3 MuzzlePosition
        {
            get
            {
                if (weaponView == null)
                    return transform.position;

                if (weaponView.MuzzlePoint == null)
                    return transform.position;

                return weaponView.MuzzlePoint.position;
            }
        }
    }
}
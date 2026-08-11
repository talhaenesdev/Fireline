using FireLine.Scripts.Weapon.Model;
using FireLine.Scripts.Weapon.View;
using UnityEngine;

namespace FireLine.Scripts.Weapon.Controller
{
    public class WeaponController : MonoBehaviour
    {
        [SerializeField] private WeaponView view;

        private WeaponModel model;

        private float nextFireTime;

        private void Awake()
        {
            model = new WeaponModel(
                damage: 25,
                fireRate: 0.25f,
                bulletSpeed: 20f
            );
        }

        public void TryFire()
        {
            if (Time.time < nextFireTime)
                return;

            nextFireTime = Time.time + model.FireRate;

            Fire();
        }

        private void Fire()
        {
            view.PlayFireEffect();

            Debug.Log("BANG!");
        }
    }
}

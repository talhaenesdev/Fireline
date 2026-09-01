using UnityEngine;

namespace FireLine.Scripts.Weapon.Model
{
    public class WeaponModel
    {
        public int Damage { get; private set; }
        public float FireRate { get; private set; }
        public float BulletSpeed { get; private set; }

        public WeaponModel(
            int damage,
            float fireRate,
            float bulletSpeed)
        {
            Damage = damage;
            FireRate = fireRate;
            BulletSpeed = bulletSpeed;
        }
    }

}

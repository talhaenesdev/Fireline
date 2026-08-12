using UnityEngine;

namespace FireLine.Scripts.Weapon.Model
{
    [CreateAssetMenu(
        fileName = "WeaponData",
        menuName = "FireLine/Weapon/Weapon Data"
    )]
    public class WeaponData : ScriptableObject
    {
        [Header("Weapon")]
        [SerializeField] private string weaponId;

        [Header("Projectile")]
        [SerializeField] private BulletData bulletData;

        [Header("Fire")]
        [SerializeField] private float fireRate = 5f;

        public string WeaponId => weaponId;
        public BulletData BulletData => bulletData;
        public float FireRate => fireRate;
    }
}
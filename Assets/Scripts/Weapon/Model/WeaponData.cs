using UnityEngine;
using FireLine.Scripts.Weapon.Model;

namespace FireLine.Scripts.Weapon.Model
{
    [CreateAssetMenu(
        fileName = "WeaponData",
        menuName = "FireLine/Weapon/Weapon Data"
    )]
    public class WeaponData : ScriptableObject
    {
        [SerializeField]
        private BulletData bulletData;

        [SerializeField]
        private float fireRate = 0.25f;

        public BulletData BulletData => bulletData;
        public float FireRate => fireRate;
    }
}
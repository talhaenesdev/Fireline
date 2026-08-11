using UnityEngine;

namespace FireLine.Scripts.Weapon.View
{
    public class WeaponView : MonoBehaviour
    {
        [SerializeField] private Transform firePoint;

        public Transform FirePoint => firePoint;

        public void PlayFireEffect()
        {
            // Muzzle flash / sound / animation
        }
    }

}

using UnityEngine;

namespace FireLine.Scripts.Weapon.View
{
    public class WeaponView : MonoBehaviour
    {
        [SerializeField]
        private Transform muzzlePoint;

        public Transform MuzzlePoint => muzzlePoint;
    }
}
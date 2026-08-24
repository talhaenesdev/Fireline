using UnityEngine;

namespace FireLine.Scripts.Core.Weapon
{
    public interface IWeaponFireService
    {
        void Fire(
            Vector3 position,
            Vector3 direction);
    }
}
using UnityEngine;

namespace Modules.WeaponModule.Controllers
{
    /// <summary>One round's direction: the aim turned by a random amount within the weapon's spread.</summary>
    internal class WeaponSpreadFunction
    {
        public Vector3 Execute(Vector3 direction, float spreadAngle) =>
            spreadAngle <= 0f ? direction : Quaternion.Euler(0f, Random.Range(-spreadAngle, spreadAngle), 0f) * direction;
    }
}

using System.Collections.Generic;
using UnityEngine;

namespace Modules.WeaponModule.Data.UnityObjects
{
    /// <summary>Every weapon a player can carry; element 0 is the one each player starts with.</summary>
    [CreateAssetMenu(fileName = "CD_WeaponSet", menuName = "Fireline/Weapon/CD_WeaponSet")]
    public class CD_WeaponSet : ScriptableObject
    {
        [SerializeField] private List<CD_Weapon> _weapons = new();

        public int Count => _weapons.Count;
        public CD_Weapon Get(int index) => index >= 0 && index < _weapons.Count ? _weapons[index] : null;
    }
}

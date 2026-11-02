using System;
using Modules.EconomyModule.Shared.Enums;
using UnityEngine;

namespace Modules.EconomyModule.Shared.Data.ValueObjects
{
    /// <summary>One thing the shop sells: its name, price, what it does and how much of it.</summary>
    [Serializable]
    public class ShopItemCVO
    {
        public string Name;
        public int Price;

        [Tooltip("Materials it takes besides the money; zero for none.")]
        public int MaterialCost;
        public ShopItemType Type;

        [Tooltip("Heal and Shield: points. FireRate: how much shorter the time between shots gets, 0.15 is 15%.")]
        public float Amount;

        [Tooltip("Weapon: which weapon of the weapon set it gives. Perk: 0 more health, 1 faster revives, 2 faster feet. Ammo: 0 for the weapon in hand, or one past a weapon's place in the set for that weapon only.")]
        public int Index;
    }
}

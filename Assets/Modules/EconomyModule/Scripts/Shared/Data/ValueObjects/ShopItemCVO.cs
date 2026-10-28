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
        public ShopItemType Type;

        [Tooltip("Heal and Shield: points. FireRate: how much shorter the time between shots gets, 0.15 is 15%.")]
        public float Amount;
    }
}

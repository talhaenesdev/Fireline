using System;
using UnityEngine;

namespace Modules.EconomyModule.Data.ValueObjects
{
    /// <summary>A kind of scrap a zombie can drop, what it sells for and how often it drops.</summary>
    [Serializable]
    public class LootItemCVO
    {
        public string Name;
        public int SellPrice = 10;
        [Min(0f)] public float Weight = 1f;
        public Color Color = Color.yellow;
    }
}

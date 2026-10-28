using System.Collections.Generic;
using Modules.EconomyModule.Data.ValueObjects;
using UnityEngine;

namespace Modules.EconomyModule.Data.UnityObjects
{
    /// <summary>What zombies drop and how it is picked up and sold.</summary>
    [CreateAssetMenu(fileName = "CD_Loot", menuName = "Fireline/Economy/CD_Loot")]
    public class CD_Loot : ScriptableObject
    {
        [SerializeField, Range(0f, 1f)] private float _dropChance = 0.6f;
        [SerializeField] private float _pickupRadius = 1.6f;
        [SerializeField] private List<LootItemCVO> _items = new();

        public float DropChance => _dropChance;
        public float PickupRadius => _pickupRadius;
        public IReadOnlyList<LootItemCVO> Items => _items;
    }
}

using System.Collections.Generic;
using Modules.EconomyModule.Shared.Data.ValueObjects;
using UnityEngine;

namespace Modules.EconomyModule.Data.UnityObjects
{
    /// <summary>What the shop sells, in the order it lists them.</summary>
    [CreateAssetMenu(fileName = "CD_Shop", menuName = "Fireline/Economy/CD_Shop")]
    public class CD_Shop : ScriptableObject
    {
        [SerializeField] private List<ShopItemCVO> _items = new();

        public IReadOnlyList<ShopItemCVO> Items => _items;
    }
}

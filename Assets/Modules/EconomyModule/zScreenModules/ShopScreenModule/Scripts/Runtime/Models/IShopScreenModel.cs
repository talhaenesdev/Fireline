using System;
using Modules.EconomyModule.Shared.Data.ValueObjects;

namespace Modules.EconomyModule.ShopScreenModule.Models
{
    /// <summary>What the shop sells and the money last reported, for a screen that opens after either arrived.</summary>
    internal interface IShopScreenModel
    {
        ShopItemCVO[] Items { get; set; }
        int Money { get; set; }
    }

    internal class ShopScreenModel : IShopScreenModel
    {
        public ShopItemCVO[] Items { get; set; } = Array.Empty<ShopItemCVO>();
        public int Money { get; set; }
    }
}

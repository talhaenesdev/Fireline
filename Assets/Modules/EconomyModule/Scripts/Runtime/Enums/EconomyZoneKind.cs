namespace Modules.EconomyModule.Enums
{
    public enum EconomyZoneKind
    {
        Sell = 0,
        Shop = 1,

        /// <summary>Opens like a shop, with its own stock: perks, each bought once.</summary>
        PerkMachine = 2,

        /// <summary>A locked door: its stock is the price of opening it, and once open it sells nothing.</summary>
        Door = 3
    }
}

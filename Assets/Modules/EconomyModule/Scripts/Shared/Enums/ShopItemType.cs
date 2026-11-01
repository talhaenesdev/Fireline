namespace Modules.EconomyModule.Shared.Enums
{
    /// <summary>What a shop item does for the player who buys it.</summary>
    public enum ShopItemType
    {
        Heal = 0,
        Shield = 1,
        FireRate = 2,
        Turret = 3,
        Weapon = 4,
        TurretUpgrade = 5,
        Barricade = 6,

        /// <summary>A perk the player keeps for the match; Index names it, Amount is how strong. Bought once.</summary>
        Perk = 7,

        /// <summary>Reloads take this share less time. Bought once.</summary>
        ReloadPerk = 8,

        /// <summary>Opens the door whose zone sells it, for everyone.</summary>
        Door = 9,

        /// <summary>A wall the buyer carries and puts down; zombies have to break it.</summary>
        Wall = 10,

        /// <summary>A barracks the buyer puts down; it trains soldiers who fight for them.</summary>
        Barracks = 11,

        /// <summary>A gate the buyer puts down: zombies have to break it, players walk through.</summary>
        Gate = 12,

        /// <summary>A station that slowly mends the structures around it.</summary>
        RepairStation = 13,

        /// <summary>Every wall goes up a tier: wood, stone, metal.</summary>
        WallUpgrade = 14,

        /// <summary>Razor wire the buyer lays down.</summary>
        SpikeTrap = 15,

        /// <summary>A mine the buyer lays down.</summary>
        Mine = 16,

        /// <summary>Every barracks of the buyer goes up a level.</summary>
        BarracksUpgrade = 17
    }
}

namespace Modules.EnemyModule.Enums
{
    /// <summary>What a player can put down in the arena.</summary>
    public enum StructureKind
    {
        /// <summary>Fires at the nearest zombie in range.</summary>
        Turret = 0,

        /// <summary>Stands in the way; zombies that reach it have to break it.</summary>
        Barricade = 1,

        /// <summary>A heavier barricade; the base's own walls are these.</summary>
        Wall = 2,

        /// <summary>Trains soldiers who guard it and shoot zombies for its owner.</summary>
        Barracks = 3,

        /// <summary>The base's heart: the match is lost when zombies bring it down.</summary>
        Headquarters = 4,

        /// <summary>Stops zombies like a wall, but players walk through it.</summary>
        Gate = 5,

        /// <summary>Slowly mends every structure around it.</summary>
        RepairStation = 6,

        /// <summary>Razor wire on the ground: zombies on it slow down and bleed. It wears out.</summary>
        SpikeTrap = 7,

        /// <summary>Blows up under the first zombie that steps near it.</summary>
        Mine = 8
    }
}

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
        Barracks = 3
    }
}

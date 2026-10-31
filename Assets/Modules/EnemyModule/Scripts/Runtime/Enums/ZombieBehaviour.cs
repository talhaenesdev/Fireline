namespace Modules.EnemyModule.Enums
{
    /// <summary>How a kind of zombie fights.</summary>
    public enum ZombieBehaviour
    {
        /// <summary>Walks up and bites.</summary>
        Melee = 0,

        /// <summary>Runs up and blows itself up, hurting everything around it.</summary>
        Exploder = 1,

        /// <summary>Keeps its distance and spits at the nearest player.</summary>
        Spitter = 2,

        /// <summary>Bites, and every so often slams the ground around it.</summary>
        Slammer = 3
    }
}

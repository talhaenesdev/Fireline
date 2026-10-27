namespace Modules.MatchModule.Shared.Enums
{
    /// <summary>What a match is played as. The host picks it in the lobby; the session carries it to the match.</summary>
    public enum GameMode
    {
        /// <summary>Players against each other.</summary>
        Deathmatch = 0,

        /// <summary>Players together against waves of zombies.</summary>
        Zombie = 1
    }
}

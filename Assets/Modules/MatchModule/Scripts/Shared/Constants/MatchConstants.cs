namespace Modules.MatchModule.Shared.Constants
{
    public static class MatchConstants
    {
        /// <summary>The scene a player returns to on leaving the match.</summary>
        public const string MAIN_SCENE = "MainScene";

        public const string DEFAULT_PLAYER_NAME = "Player";

        /// <summary>The session property the host writes the chosen GameMode into.</summary>
        public const string GAME_MODE_PROPERTY = "GameMode";

        /// <summary>The session property the host writes the chosen Difficulty into.</summary>
        public const string DIFFICULTY_PROPERTY = "Difficulty";

        /// <summary>The attacker id a zombie hits with: no client has it, so it scores for nobody.</summary>
        public const ulong ZOMBIE_ATTACKER_ID = ulong.MaxValue;
    }
}

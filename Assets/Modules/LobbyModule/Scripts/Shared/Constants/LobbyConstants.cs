namespace Modules.LobbyModule.Shared.Constants
{
    public static class LobbyConstants
    {
        public const int MAX_PLAYERS = 4;

        /// <summary>The scene Netcode loads on every client when the host starts the match.</summary>
        public const string GAME_SCENE = "Game";

        /// <summary>The flat arena a zombie match is played on.</summary>
        public const string ZOMBIE_SCENE = "ZombieArena";

        public const string STATUS_IDLE = "Ready to create or join a lobby.";
        public const string STATUS_CREATING = "Creating lobby...";
        public const string STATUS_CREATED = "Lobby created successfully!";
        public const string STATUS_CREATE_FAILED = "Failed to create lobby.";
        public const string STATUS_JOINING = "Joining lobby...";
        public const string STATUS_JOINED = "Successfully joined the lobby!";
        public const string STATUS_JOIN_FAILED = "Failed to join lobby.";
        public const string STATUS_LEAVING = "Leaving lobby...";
        public const string STATUS_LEFT = "You left the lobby.";
        public const string STATUS_STARTING = "Starting game...";
        public const string STATUS_START_FAILED = "Failed to start the game.";
        public const string STATUS_NAME_MISSING = "Please enter your player name.";
        public const string STATUS_CODE_MISSING = "Please enter a join code.";
        public const string STATUS_SERVICES_FAILED = "Could not reach Unity Services.";
    }
}

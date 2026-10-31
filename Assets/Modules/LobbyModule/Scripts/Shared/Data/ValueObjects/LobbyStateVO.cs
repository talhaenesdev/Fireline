using Modules.LobbyModule.Shared.Enums;
using Modules.MatchModule.Shared.Enums;

namespace Modules.LobbyModule.Shared.Data.ValueObjects
{
    /// <summary>Everything the lobby panel draws, in one value so it is redrawn in one pass.</summary>
    public readonly struct LobbyStateVO
    {
        public readonly bool HasSession;
        public readonly bool IsHost;
        public readonly string JoinCode;
        public readonly int PlayerCount;
        public readonly int MaxPlayers;
        public readonly bool IsLocalReady;
        public readonly bool AreAllReady;
        public readonly bool IsBusy;
        public readonly string Status;
        public readonly GameMode Mode;
        public readonly Difficulty Difficulty;
        public readonly ZombieMap Map;

        public LobbyStateVO(bool hasSession, bool isHost, string joinCode, int playerCount, int maxPlayers,
            bool isLocalReady, bool areAllReady, bool isBusy, string status, GameMode mode, Difficulty difficulty, ZombieMap map)
        {
            HasSession = hasSession;
            IsHost = isHost;
            JoinCode = joinCode;
            PlayerCount = playerCount;
            MaxPlayers = maxPlayers;
            IsLocalReady = isLocalReady;
            AreAllReady = areAllReady;
            IsBusy = isBusy;
            Status = status;
            Mode = mode;
            Difficulty = difficulty;
            Map = map;
        }

        public bool CanStart => HasSession && IsHost && AreAllReady && !IsBusy;
    }
}

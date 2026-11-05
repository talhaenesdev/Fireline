using System;
using System.Collections.Generic;

namespace Modules.NetworkSessionModule.Shared.Data.ValueObjects
{
    /// <summary>What a lobby screen needs to know about the session the local player is in.</summary>
    public readonly struct SessionStateVO
    {
        public readonly bool HasSession;
        public readonly bool IsHost;
        public readonly string JoinCode;
        public readonly int PlayerCount;
        public readonly int MaxPlayers;
        public readonly bool IsLocalReady;
        public readonly bool AreAllReady;
        /// <summary>Whether each player is ready, in the order they joined.</summary>
        public readonly IReadOnlyList<bool> PlayerReady;

        public SessionStateVO(bool hasSession, bool isHost, string joinCode, int playerCount, int maxPlayers,
            bool isLocalReady, bool areAllReady, IReadOnlyList<bool> playerReady)
        {
            HasSession = hasSession;
            IsHost = isHost;
            JoinCode = joinCode;
            PlayerCount = playerCount;
            MaxPlayers = maxPlayers;
            IsLocalReady = isLocalReady;
            AreAllReady = areAllReady;
            PlayerReady = playerReady ?? Array.Empty<bool>();
        }

        public static SessionStateVO None => new(false, false, null, 0, 0, false, false, Array.Empty<bool>());
    }
}

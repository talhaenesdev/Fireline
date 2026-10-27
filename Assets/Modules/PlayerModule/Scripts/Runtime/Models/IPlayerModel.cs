using System.Collections.Generic;
using Modules.PlayerModule.Entities;
using UnityEngine;

namespace Modules.PlayerModule.Models
{
    internal interface IPlayerModel
    {
        /// <summary>Every avatar this process knows, by owning client.</summary>
        IReadOnlyDictionary<ulong, IPlayerAvatar> Avatars { get; }

        /// <summary>The avatar this process controls, or null between death and respawn.</summary>
        IPlayerAvatar LocalAvatar { get; }

        bool IsLocalAlive { get; set; }
        Vector2 MoveInput { get; set; }
        bool IsSprinting { get; set; }

        /// <summary>Whether a player's damage reaches another player. Off in a zombie match.</summary>
        bool IsFriendlyFire { get; set; }

        /// <summary>The local player plays on a touchscreen: on-screen controls, and aiming by itself.</summary>
        bool IsTouchInput { get; set; }

        bool IsTicking { get; set; }
        bool IsHooked { get; set; }

        void Add(IPlayerAvatar avatar);
        void Remove(IPlayerAvatar avatar);
        bool TryGet(ulong clientId, out IPlayerAvatar avatar);
    }
}

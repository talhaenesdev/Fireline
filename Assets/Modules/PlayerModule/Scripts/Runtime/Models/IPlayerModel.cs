using System.Collections.Generic;
using Modules.PlayerModule.Data.UnityObjects;
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

        /// <summary>Pings shown on this process, and until when.</summary>
        System.Collections.Generic.List<(UnityEngine.Vector3 point, float until)> Pings { get; }

        /// <summary>When the HUD's markers are next gathered.</summary>
        float NextMarkersTime { get; set; }

        /// <summary>The point the local player aims at, for a ping.</summary>
        UnityEngine.Vector3 AimPoint { get; set; }

        /// <summary>The local player's health as last published; below zero before the first.</summary>
        float LastLocalHealth { get; set; }
        Vector2 MoveInput { get; set; }
        bool IsSprinting { get; set; }

        /// <summary>Whether a player's damage reaches another player. Off in a zombie match.</summary>
        bool IsFriendlyFire { get; set; }

        /// <summary>The local player plays on a touchscreen: on-screen controls, and aiming by itself.</summary>
        bool IsTouchInput { get; set; }

        /// <summary>The local camera's config, once the camera is up; its ranges bound the settings.</summary>
        CD_CameraSettings CameraConfig { get; set; }
        float CameraHeight { get; set; }
        float CameraFollowSpeed { get; set; }
        float CameraShake { get; set; }

        /// <summary>The downed teammate the local player is next to, or null.</summary>
        IPlayerAvatar ReviveTarget { get; set; }

        /// <summary>Time.time the local player's revive completes, or zero when none is under way.</summary>
        float ReviveEndTime { get; set; }

        /// <summary>Server: revives under way, by reviver, with the downed player and the time they finish.</summary>
        Dictionary<ulong, (ulong target, float endTime)> Revives { get; }

        bool IsTicking { get; set; }
        bool IsHooked { get; set; }

        void Add(IPlayerAvatar avatar);
        void Remove(IPlayerAvatar avatar);
        bool TryGet(ulong clientId, out IPlayerAvatar avatar);
    }
}

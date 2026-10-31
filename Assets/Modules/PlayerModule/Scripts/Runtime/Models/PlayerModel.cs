using System.Collections.Generic;
using Modules.PlayerModule.Data.UnityObjects;
using Modules.PlayerModule.Entities;
using UnityEngine;

namespace Modules.PlayerModule.Models
{
    internal class PlayerModel : IPlayerModel
    {
        private readonly Dictionary<ulong, IPlayerAvatar> _avatars = new();

        public IReadOnlyDictionary<ulong, IPlayerAvatar> Avatars => _avatars;
        public IPlayerAvatar LocalAvatar { get; private set; }

        public bool IsLocalAlive { get; set; }
        public List<(Vector3 point, float until)> Pings { get; } = new();
        public float NextMarkersTime { get; set; }
        public Vector3 AimPoint { get; set; }
        public float LastLocalHealth { get; set; } = -1f;
        public Vector2 MoveInput { get; set; }
        public bool IsSprinting { get; set; }

        public bool IsFriendlyFire { get; set; } = true;
        public bool IsTouchInput { get; set; }
        public CD_CameraSettings CameraConfig { get; set; }
        public float CameraHeight { get; set; }
        public float CameraFollowSpeed { get; set; }
        public float CameraShake { get; set; }
        public IPlayerAvatar ReviveTarget { get; set; }
        public float ReviveEndTime { get; set; }
        public Dictionary<ulong, (ulong target, float endTime)> Revives { get; } = new();
        public bool IsTicking { get; set; }
        public bool IsHooked { get; set; }

        public void Add(IPlayerAvatar avatar)
        {
            _avatars[avatar.ClientId] = avatar;

            if (avatar.IsOwner)
                LocalAvatar = avatar;
        }

        public void Remove(IPlayerAvatar avatar)
        {
            // A respawn adds the new avatar before the old one is gone, so only the same one is removed.
            if (_avatars.TryGetValue(avatar.ClientId, out IPlayerAvatar stored) && stored == avatar)
                _avatars.Remove(avatar.ClientId);

            if (LocalAvatar == avatar)
                LocalAvatar = null;
        }

        public bool TryGet(ulong clientId, out IPlayerAvatar avatar) => _avatars.TryGetValue(clientId, out avatar);
    }
}

using FlowIoC.BaseModule.Signals;
using Modules.PlayerModule.Shared.Data.ValueObjects;
using UnityEngine;

namespace Modules.PlayerModule.Signals
{
    public class PlayerSignals : ISignalHolder
    {
        public PlayerSignalsIncoming Incoming = new();
        public PlayerSignalsOutgoing Outgoing = new();
    }

    public class PlayerSignalsIncoming
    {
        /// <summary>Target client, damage, attacking client. Applied on the server only.</summary>
        public Signal<ulong, int, ulong> ApplyDamage = new();

        /// <summary>The local player's weapon fired.</summary>
        public Signal ShakeCamera = new();

        /// <summary>Whether one player's rounds hurt another. Server only.</summary>
        public Signal<bool> SetFriendlyFire = new();

        /// <summary>The on-screen stick of a phone, in place of the keyboard.</summary>
        public Signal<Vector2> TouchMove = new(hideCommandLog: true);

        /// <summary>Client id, points of health given back. Server only.</summary>
        public Signal<ulong, float> Heal = new();

        /// <summary>Client id, points of shield added. Server only.</summary>
        public Signal<ulong, float> AddShield = new();

        /// <summary>The local player pressed revive on the downed teammate next to it.</summary>
        public Signal StartRevive = new();

        /// <summary>The local camera's height, follow speed and shake, as a player set them.</summary>
        public Signal<float, float, float> SetCameraSettings = new();
    }

    public class PlayerSignalsOutgoing
    {
        /// <summary>Held, and pressed this frame. Local player only.</summary>
        public Signal<bool, bool> FireInput = new();

        public Signal ReloadInput = new();

        /// <summary>
        /// The point on the ground the local player aims at, every frame it changes. A point, not a
        /// direction: the gun sits off to the avatar's side, so a round sent along the avatar's facing
        /// would pass beside what it was aimed at.
        /// </summary>
        public Signal<Vector3> AimChanged = new(hideCommandLog: true);

        /// <summary>The local player spawned, died or came back.</summary>
        public Signal<bool> LocalAliveChanged = new();

        /// <summary>The local player's shield points.</summary>
        public Signal<float> LocalShieldChanged = new();

        /// <summary>
        /// Whether a downed teammate is close enough to revive, where it lies, and how far the
        /// revive has got from 0 to 1, or below zero when none is under way. Every frame. Local only.
        /// </summary>
        public Signal<bool, Vector3, float> ReviveStateChanged = new(hideCommandLog: true);

        /// <summary>A downed player is back on their feet. Server only.</summary>
        public Signal<ulong> PlayerRevived = new();

        /// <summary>Current and maximum health of the local player.</summary>
        public Signal<float, float> LocalHealthChanged = new();

        /// <summary>Victim, then killer. Server only.</summary>
        public Signal<ulong, ulong> PlayerDied = new();

        /// <summary>A player's name was accepted. Server only.</summary>
        public Signal<ulong, string> PlayerNameChanged = new();

        /// <summary>The local camera's settings and their ranges, when it comes up and whenever they change.</summary>
        public Signal<CameraSettingsVO> CameraSettingsChanged = new();
    }
}

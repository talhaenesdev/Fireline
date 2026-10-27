using FlowIoC.BaseModule.Signals;
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
    }

    public class PlayerSignalsOutgoing
    {
        /// <summary>Held, and pressed this frame. Local player only.</summary>
        public Signal<bool, bool> FireInput = new();

        public Signal ReloadInput = new();

        /// <summary>The local player's aim on the ground plane, every frame it changes.</summary>
        public Signal<Vector3> AimChanged = new(hideCommandLog: true);

        /// <summary>The local player spawned, died or came back.</summary>
        public Signal<bool> LocalAliveChanged = new();

        /// <summary>Current and maximum health of the local player.</summary>
        public Signal<float, float> LocalHealthChanged = new();

        /// <summary>Victim, then killer. Server only.</summary>
        public Signal<ulong, ulong> PlayerDied = new();

        /// <summary>A player's name was accepted. Server only.</summary>
        public Signal<ulong, string> PlayerNameChanged = new();
    }
}

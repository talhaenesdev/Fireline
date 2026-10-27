using FlowIoC.BaseModule.Signals;
using UnityEngine;

namespace Modules.WeaponModule.Signals
{
    public class WeaponSignals : ISignalHolder
    {
        public WeaponSignalsIncoming Incoming = new();
        public WeaponSignalsOutgoing Outgoing = new();
    }

    public class WeaponSignalsIncoming
    {
        /// <summary>The trigger is held, and it was pressed this frame.</summary>
        public Signal<bool, bool> FireInput = new();

        public Signal ReloadInput = new();

        public Signal<Vector3> AimChanged = new(hideCommandLog: true);

        /// <summary>Whether the local player may fire at all: alive and spawned.</summary>
        public Signal<bool> SetArmed = new();
    }

    public class WeaponSignalsOutgoing
    {
        /// <summary>Rounds left, then the magazine's size, of the local weapon.</summary>
        public Signal<int, int> AmmoChanged = new();

        /// <summary>The local weapon fired a round.</summary>
        public Signal LocalFired = new();

        /// <summary>Hit client, damage, shooting client. Server only.</summary>
        public Signal<ulong, int, ulong> PlayerHit = new();

        /// <summary>A networked object that is no player was hit: its NetworkObjectId, damage, shooting client. Server only.</summary>
        public Signal<ulong, int, ulong> ObjectHit = new();
    }
}

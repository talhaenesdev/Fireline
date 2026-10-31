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

        /// <summary>The point on the ground the local player aims at; rounds leave the muzzle towards it.</summary>
        public Signal<Vector3> AimChanged = new(hideCommandLog: true);

        /// <summary>The local weapon fires faster: each shot's wait gets this much shorter, 0.15 is 15%.</summary>
        public Signal<float> ImproveFireRate = new();

        /// <summary>The local weapon reloads faster: the share of the time taken off, 0.4 is 40%.</summary>
        public Signal<float> ImproveReload = new();

        /// <summary>Server: this player now carries this weapon of the set: client id, index.</summary>
        public Signal<ulong, int> EquipWeapon = new();

        /// <summary>Whether the local player may fire at all: alive and spawned.</summary>
        public Signal<bool> SetArmed = new();
    }

    public class WeaponSignalsOutgoing
    {
        /// <summary>Rounds left, then the magazine's size, of the local weapon.</summary>
        public Signal<int, int> AmmoChanged = new();

        /// <summary>The local player carries a new weapon: its name.</summary>
        public Signal<string> LocalWeaponChanged = new();

        /// <summary>The local weapon fired a round.</summary>
        public Signal LocalFired = new();

        /// <summary>Hit client, damage, shooting client. Server only.</summary>
        public Signal<ulong, int, ulong> PlayerHit = new();

        /// <summary>A networked object that is no player was hit: its NetworkObjectId, damage, shooting client. Server only.</summary>
        public Signal<ulong, int, ulong> ObjectHit = new();
    }
}

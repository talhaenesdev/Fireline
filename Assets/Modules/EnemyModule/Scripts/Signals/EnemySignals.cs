using FlowIoC.BaseModule.Signals;
using UnityEngine;

namespace Modules.EnemyModule.Signals
{
    public class EnemySignals : ISignalHolder
    {
        public EnemySignalsIncoming Incoming = new();
        public EnemySignalsOutgoing Outgoing = new();
    }

    public class EnemySignalsIncoming
    {
        /// <summary>The match is a zombie match: begin the waves. Server only.</summary>
        public Signal StartWaves = new();

        /// <summary>The match is over: no more waves. Server only.</summary>
        public Signal StopWaves = new();

        /// <summary>A networked object was hit: its NetworkObjectId, damage, shooting client. Server only.</summary>
        public Signal<ulong, int, ulong> DamageZombie = new();

        /// <summary>Victim, then killer. A downed player is no longer chased. Server only.</summary>
        public Signal<ulong, ulong> PlayerDied = new();

        /// <summary>A downed player is back up and is a target again. Server only.</summary>
        public Signal<ulong> PlayerRevived = new();
    }

    public class EnemySignalsOutgoing
    {
        /// <summary>A zombie reached a player: target client, damage, attacker id. Server only.</summary>
        public Signal<ulong, int, ulong> PlayerAttacked = new();

        /// <summary>A zombie went down to the shooting client. Server only.</summary>
        public Signal<ulong> ZombieKilled = new();

        /// <summary>Where a zombie fell. Server only.</summary>
        public Signal<Vector3> ZombieDied = new();

        /// <summary>
        /// The wave, the seconds until it comes in (zero once it is on), and the zombies still
        /// standing. On every client.
        /// </summary>
        public Signal<int, float, int> WaveStateChanged = new();
    }
}

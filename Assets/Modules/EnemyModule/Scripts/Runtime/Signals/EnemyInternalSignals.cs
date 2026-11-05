using FlowIoC.BaseModule.Signals;
using Modules.EnemyModule.Entities;
using UnityEngine;

namespace Modules.EnemyModule.Signals
{
    internal class EnemyInternalSignals : ISignalHolder
    {
        /// <summary>Every process: a zombie was hit here.</summary>
        public Signal<Vector3> ZombieHitShown = new(hideCommandLog: true);

        /// <summary>Every process: a zombie fell here.</summary>
        public Signal<Vector3> ZombieDeathShown = new();

        /// <summary>Every process: something blew up here.</summary>
        public Signal<Vector3> BlastShown = new();

        /// <summary>Every process: a structure came down here.</summary>
        public Signal<Vector3> StructureFellShown = new();

        /// <summary>Every process: a wave was cleared.</summary>
        public Signal WaveClearedShown = new();

        public Signal<IZombieWaveBoard> BoardRegistered = new();

        /// <summary>A board registered and needs the pool before it draws anything.</summary>
        public Signal<IZombieWaveBoard> PoolRequested = new();

        /// <summary>The module launched: every Root has set up, and with them the pool groups.</summary>
        public Signal Launched = new();

        /// <summary>Server: a player asked to mend the structure they stand by.</summary>
        public Signal<ulong> RepairRequested = new();

        /// <summary>Server: a player asked to switch their soldiers between guarding and following.</summary>
        public Signal<ulong> AllyOrderRequested = new();

        /// <summary>Server: a player asked for the next wave now.</summary>
        public Signal SkipBreakRequested = new();

        /// <summary>Buyer's process: the server gave the local player a structure of this kind to place.</summary>
        public Signal<int> StructureGranted = new();

        /// <summary>Server: a client wants its structure here: client id, position, kind.</summary>
        public Signal<ulong, Vector3, int> StructurePlaceRequested = new();

        /// <summary>One turn of the waves: wait, spawn, wait until cleared, go again.</summary>
        public Signal NextWave = new();

        /// <summary>The zombies' frame: chase and bite.</summary>
        public Signal Tick = new(hideCommandLog: true);
    }
}

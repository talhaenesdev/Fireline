using FlowIoC.BaseModule.Signals;
using Modules.EnemyModule.Entities;
using UnityEngine;

namespace Modules.EnemyModule.Signals
{
    internal class EnemyInternalSignals : ISignalHolder
    {
        public Signal<IZombieWaveBoard> BoardRegistered = new();

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

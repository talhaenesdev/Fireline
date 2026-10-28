using FlowIoC.BaseModule.Signals;
using Modules.EnemyModule.Entities;
using UnityEngine;

namespace Modules.EnemyModule.Signals
{
    internal class EnemyInternalSignals : ISignalHolder
    {
        public Signal<IZombieWaveBoard> BoardRegistered = new();

        /// <summary>Buyer's process: the server gave the local player a turret to place.</summary>
        public Signal TurretGranted = new();

        /// <summary>Server: a client wants its turret here: client id, position.</summary>
        public Signal<ulong, Vector3> TurretPlaceRequested = new();

        /// <summary>One turn of the waves: wait, spawn, wait until cleared, go again.</summary>
        public Signal NextWave = new();

        /// <summary>The zombies' frame: chase and bite.</summary>
        public Signal Tick = new(hideCommandLog: true);
    }
}

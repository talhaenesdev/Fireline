using FlowIoC.BaseModule.Signals;
using Modules.EnemyModule.Entities;

namespace Modules.EnemyModule.Signals
{
    internal class EnemyInternalSignals : ISignalHolder
    {
        public Signal<IZombieWaveBoard> BoardRegistered = new();

        /// <summary>One turn of the waves: wait, spawn, wait until cleared, go again.</summary>
        public Signal NextWave = new();

        /// <summary>The zombies' frame: chase and bite.</summary>
        public Signal Tick = new(hideCommandLog: true);
    }
}

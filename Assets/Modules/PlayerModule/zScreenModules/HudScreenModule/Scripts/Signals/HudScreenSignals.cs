using FlowIoC.BaseModule.Signals;
using UnityEngine;

namespace Modules.PlayerModule.HudScreenModule.Signals
{
    public class HudScreenSignals : ISignalHolder
    {
        public HudScreenSignalsIncoming Incoming = new();
        public HudScreenSignalsOutgoing Outgoing = new();
    }

    public class HudScreenSignalsIncoming
    {
        public Signal Open = new();

        /// <summary>Current and maximum health.</summary>
        public Signal<float, float> ApplyHealth = new();

        /// <summary>Rounds left, then the magazine's size.</summary>
        public Signal<int, int> ApplyAmmo = new();
    }

    public class HudScreenSignalsOutgoing
    {
        /// <summary>The on-screen stick, on a phone.</summary>
        public Signal<Vector2> TouchMove = new(hideCommandLog: true);

        /// <summary>The on-screen trigger is held, and it was pressed just now.</summary>
        public Signal<bool, bool> TouchFire = new();

        public Signal TouchReload = new();
    }
}

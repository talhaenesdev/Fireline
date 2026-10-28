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

        /// <summary>The wave, the seconds until it comes in (zero once it is on), and the zombies left.</summary>
        public Signal<int, float, int> ApplyWave = new();

        /// <summary>Shield points of the local player.</summary>
        public Signal<float> ApplyShield = new();

        /// <summary>Money, then how many pieces of loot are carried.</summary>
        public Signal<int, int> ApplyWallet = new();

        /// <summary>A downed teammate in reach, where it lies, and the revive's progress (below zero: none).</summary>
        public Signal<bool, Vector3, float> ApplyRevive = new(hideCommandLog: true);

        /// <summary>The local player has a turret to place: show the button that puts it down.</summary>
        public Signal<bool> ApplyTurretPlacement = new();
    }

    public class HudScreenSignalsOutgoing
    {
        /// <summary>The on-screen stick, on a phone.</summary>
        public Signal<Vector2> TouchMove = new(hideCommandLog: true);

        /// <summary>The on-screen trigger is held, and it was pressed just now.</summary>
        public Signal<bool, bool> TouchFire = new();

        public Signal TouchReload = new();

        /// <summary>The revive button over a downed teammate was pressed.</summary>
        public Signal ReviveClicked = new();

        /// <summary>The place-turret button was pressed.</summary>
        public Signal PlaceTurretClicked = new();
    }
}

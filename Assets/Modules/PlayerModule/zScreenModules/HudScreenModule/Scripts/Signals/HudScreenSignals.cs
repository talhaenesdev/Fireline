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

        /// <summary>What to point at when it is off the screen.</summary>
        public Signal<Modules.PlayerModule.Shared.Data.ValueObjects.MarkerVO[]> ApplyMarkers = new(hideCommandLog: true);

        /// <summary>The local player's level, the experience into it, and how much the level takes.</summary>
        public Signal<int, int, int> ApplyProgress = new();

        /// <summary>The defend objective: title, whether it is on, how far it is held.</summary>
        public Signal<string, bool, float> ApplyObjective = new(hideCommandLog: true);

        /// <summary>The objective ended: held or not, and what it paid.</summary>
        public Signal<bool, int> ApplyObjectiveEnded = new();

        /// <summary>The name of the weapon the local player carries.</summary>
        public Signal<string> ApplyWeaponName = new();

        /// <summary>The local player was hurt this much: the screen's edges flash red.</summary>
        public Signal<float> ApplyHurt = new();

        /// <summary>The boss's name, health and most health; an empty name hides the bar.</summary>
        public Signal<string, int, int> ApplyBoss = new();

        /// <summary>A wave was cleared: its number, then what it paid.</summary>
        public Signal<int, int> ApplyWaveCleared = new();

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

        /// <summary>The ping button was pressed.</summary>
        public Signal PingClicked = new();

        /// <summary>The place-turret button was pressed.</summary>
        public Signal PlaceTurretClicked = new();
    }
}

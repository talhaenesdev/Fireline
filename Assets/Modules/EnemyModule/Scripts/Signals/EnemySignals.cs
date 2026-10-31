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

        /// <summary>How hard the match is: 0 easy, 1 normal, 2 nightmare. Server only.</summary>
        public Signal<int> SetDifficulty = new();

        /// <summary>The match is over: no more waves. Server only.</summary>
        public Signal StopWaves = new();

        /// <summary>A networked object was hit: its NetworkObjectId, damage, shooting client. Server only.</summary>
        public Signal<ulong, int, ulong> DamageZombie = new();

        /// <summary>Victim, then killer. A downed player is no longer chased. Server only.</summary>
        public Signal<ulong, ulong> PlayerDied = new();

        /// <summary>A player bought a turret; they now choose where it goes. Server only.</summary>
        public Signal<ulong> GrantTurret = new();

        /// <summary>A player bought a barricade; they now choose where it goes. Server only.</summary>
        public Signal<ulong> GrantBarricade = new();

        /// <summary>A player bought a wall; they now choose where it goes. Server only.</summary>
        public Signal<ulong> GrantWall = new();

        /// <summary>A player bought a barracks; they now choose where it goes. Server only.</summary>
        public Signal<ulong> GrantBarracks = new();

        /// <summary>A player bought an upgrade: each of their turrets goes up a level. Server only.</summary>
        public Signal<ulong> UpgradeTurrets = new();

        /// <summary>The local player puts the carried structure down where they stand.</summary>
        public Signal ConfirmTurretPlacement = new();

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

        /// <summary>The boss's name, health and most health; an empty name when none is standing. On every client.</summary>
        public Signal<string, int, int> BossStateChanged = new();

        /// <summary>The defend objective: its title, whether it is on, how far it is held. On every client.</summary>
        public Signal<string, bool, float> ObjectiveChanged = new(hideCommandLog: true);

        /// <summary>The objective ended: held or not, and what it paid each player. On every client.</summary>
        public Signal<bool, int> ObjectiveEnded = new();

        /// <summary>The objective was held: every player is paid this much. Server only.</summary>
        public Signal<int> ObjectiveRewarded = new();

        /// <summary>A wave was cleared: its number. On every client.</summary>
        public Signal<int> WaveCleared = new();

        /// <summary>The local player has a structure to place, or no longer has one.</summary>
        public Signal<bool> TurretPlacementChanged = new();
    }
}

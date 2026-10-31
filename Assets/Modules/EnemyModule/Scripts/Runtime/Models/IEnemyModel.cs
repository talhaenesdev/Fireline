using System.Collections.Generic;
using Modules.EnemyModule.Data.UnityObjects;
using Modules.EnemyModule.Entities;

namespace Modules.EnemyModule.Models
{
    /// <summary>The server's zombie match: the board, the wave it is on, and every zombie still standing.</summary>
    internal interface IEnemyModel
    {
        IZombieWaveBoard Board { get; set; }
        CD_ZombieWaves Config { get; }
        bool IsRunning { get; set; }
        int Wave { get; set; }

        /// <summary>The match's difficulty, as an index into the config's difficulties.</summary>
        int Difficulty { get; set; }

        /// <summary>The zombies alive, by NetworkObjectId.</summary>
        Dictionary<ulong, ZombieRVO> Zombies { get; }

        /// <summary>Players who are down; zombies leave them be.</summary>
        HashSet<ulong> DownedClients { get; }

        /// <summary>The turrets standing, by id.</summary>
        Dictionary<int, TurretRVO> Turrets { get; }
        int NextTurretId { get; set; }

        /// <summary>Server: the structures each player has bought and not put down yet.</summary>
        Dictionary<ulong, List<Enums.StructureKind>> PendingStructures { get; }

        /// <summary>The local player's structures waiting to be placed, the first one carried.</summary>
        List<Enums.StructureKind> LocalStructuresToPlace { get; }

        /// <summary>The zombie prefab Netcode now spawns from the pool, on this process; none until the board is up.</summary>
        UnityEngine.GameObject PooledZombiePrefab { get; set; }

        /// <summary>Server: the defend objective of the wave, if one is on.</summary>
        bool IsObjectiveActive { get; set; }
        UnityEngine.Vector3 ObjectivePosition { get; set; }
        float ObjectiveProgress { get; set; }

        /// <summary>Server: spit in flight.</summary>
        List<SpitRVO> Spits { get; }

        /// <summary>Server: the soldiers the barracks trained, by NetworkObjectId.</summary>
        Dictionary<ulong, AllyRVO> Allies { get; }

        /// <summary>The soldier prefab Netcode now spawns from the pool, on this process.</summary>
        UnityEngine.GameObject PooledAllyPrefab { get; set; }

        bool IsTicking { get; set; }
        bool IsHooked { get; set; }
    }

    /// <summary>A zombie's state that only the server keeps.</summary>
    internal class ZombieRVO
    {
        public Zombie Zombie;
        public Data.ValueObjects.ZombieTypeCVO Type;
        public int Health;
        public float NextAttackTime;
        public float NextAreaTime;
    }

    /// <summary>A soldier's state that only the server keeps.</summary>
    internal class AllyRVO
    {
        public Ally Ally;
        public int BarracksId;
        public ulong OwnerClientId;
        public UnityEngine.Vector3 Post;
        public int Health;
        public float NextFireTime;
    }

    /// <summary>A gob of spit the server flies until it hits a player or runs out.</summary>
    internal class SpitRVO
    {
        public UnityEngine.Vector3 Position;
        public UnityEngine.Vector3 Velocity;
        public int Damage;
        public float TimeLeft;
    }

    /// <summary>A turret's state that only the server keeps.</summary>
    internal class TurretRVO
    {
        public int Id;
        public Enums.StructureKind Kind;
        public int Level;
        public ulong OwnerClientId;
        public UnityEngine.Vector3 Position;
        public float Yaw;
        public int Health;
        public float NextFireTime;

        /// <summary>A barracks: when it trains its next soldier.</summary>
        public float NextTrainTime;
    }

    internal class EnemyModel : IEnemyModel
    {
        public IZombieWaveBoard Board { get; set; }
        public CD_ZombieWaves Config => Board?.Config;
        public bool IsRunning { get; set; }
        public int Wave { get; set; }
        public int Difficulty { get; set; } = 1;
        public Dictionary<ulong, ZombieRVO> Zombies { get; } = new();
        public HashSet<ulong> DownedClients { get; } = new();
        public Dictionary<int, TurretRVO> Turrets { get; } = new();
        public int NextTurretId { get; set; }
        public Dictionary<ulong, List<Enums.StructureKind>> PendingStructures { get; } = new();
        public List<Enums.StructureKind> LocalStructuresToPlace { get; } = new();
        public List<SpitRVO> Spits { get; } = new();
        public Dictionary<ulong, AllyRVO> Allies { get; } = new();
        public UnityEngine.GameObject PooledAllyPrefab { get; set; }
        public UnityEngine.GameObject PooledZombiePrefab { get; set; }
        public bool IsObjectiveActive { get; set; }
        public UnityEngine.Vector3 ObjectivePosition { get; set; }
        public float ObjectiveProgress { get; set; }
        public bool IsTicking { get; set; }
        public bool IsHooked { get; set; }
    }
}

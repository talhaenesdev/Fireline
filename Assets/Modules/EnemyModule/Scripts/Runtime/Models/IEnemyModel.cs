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

        /// <summary>The zombies alive, by NetworkObjectId.</summary>
        Dictionary<ulong, ZombieRVO> Zombies { get; }

        /// <summary>Players who are down; zombies leave them be.</summary>
        HashSet<ulong> DownedClients { get; }

        /// <summary>The turrets standing, by id.</summary>
        Dictionary<int, TurretRVO> Turrets { get; }
        int NextTurretId { get; set; }

        bool IsTicking { get; set; }
        bool IsHooked { get; set; }
    }

    /// <summary>A zombie's state that only the server keeps.</summary>
    internal class ZombieRVO
    {
        public Zombie Zombie;
        public int Health;
        public float NextAttackTime;
    }

    /// <summary>A turret's state that only the server keeps.</summary>
    internal class TurretRVO
    {
        public int Id;
        public ulong OwnerClientId;
        public UnityEngine.Vector3 Position;
        public float Yaw;
        public int Health;
        public float NextFireTime;
    }

    internal class EnemyModel : IEnemyModel
    {
        public IZombieWaveBoard Board { get; set; }
        public CD_ZombieWaves Config => Board?.Config;
        public bool IsRunning { get; set; }
        public int Wave { get; set; }
        public Dictionary<ulong, ZombieRVO> Zombies { get; } = new();
        public HashSet<ulong> DownedClients { get; } = new();
        public Dictionary<int, TurretRVO> Turrets { get; } = new();
        public int NextTurretId { get; set; }
        public bool IsTicking { get; set; }
        public bool IsHooked { get; set; }
    }
}

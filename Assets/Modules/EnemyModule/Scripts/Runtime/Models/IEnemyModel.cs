using System.Collections.Generic;
using Modules.EnemyModule.Data.UnityObjects;
using Modules.EnemyModule.Entities;

namespace Modules.EnemyModule.Models
{
    /// <summary>The server's zombie match: the config, the wave it is on, and every zombie still standing.</summary>
    internal interface IEnemyModel
    {
        CD_ZombieWaves Config { get; set; }
        bool IsRunning { get; set; }
        int Wave { get; set; }

        /// <summary>The zombies alive, by NetworkObjectId.</summary>
        Dictionary<ulong, ZombieRVO> Zombies { get; }

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

    internal class EnemyModel : IEnemyModel
    {
        public CD_ZombieWaves Config { get; set; }
        public bool IsRunning { get; set; }
        public int Wave { get; set; }
        public Dictionary<ulong, ZombieRVO> Zombies { get; } = new();
        public bool IsTicking { get; set; }
        public bool IsHooked { get; set; }
    }
}

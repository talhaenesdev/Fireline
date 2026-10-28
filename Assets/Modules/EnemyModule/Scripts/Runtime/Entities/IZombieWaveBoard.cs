using Modules.EnemyModule.Data.UnityObjects;
using UnityEngine;

namespace Modules.EnemyModule.Entities
{
    /// <summary>
    /// The match scene's zombie waves: their settings, the points they come in at, and the wave
    /// state every client is shown. The server writes the state; the clients only read it.
    /// </summary>
    internal interface IZombieWaveBoard
    {
        CD_ZombieWaves Config { get; }
        CD_Turret TurretConfig { get; }
        int SpawnPointCount { get; }
        Vector3 GetSpawnPoint(int index);

        /// <summary>Server only: the next wave comes in this many seconds from now.</summary>
        void SetCountdownServer(int nextWave, float seconds);

        /// <summary>Server only: a wave is on, with this many zombies still standing.</summary>
        void SetWaveServer(int wave, int zombiesLeft);

        /// <summary>Server only: a turret stands here, facing this way, with this much health left.</summary>
        void SetTurretServer(int id, Vector3 position, float yaw, int health);

        void RemoveTurretServer(int id);

        /// <summary>Server only: every client sees the turret fire at this point.</summary>
        void ShowTurretShotServer(int id, Vector3 target);

        /// <summary>Server: tells the buyer it has a turret to place.</summary>
        void GrantTurretServer(ulong clientId);

        /// <summary>Client: asks the server to put the local player's turret here.</summary>
        void RequestPlaceTurret(Vector3 position);

        /// <summary>Client: the local preview of the turret being placed, green where it may go; hidden when not shown.</summary>
        void ShowPlacementPreview(bool isShown, Vector3 position, bool isValid);
    }
}

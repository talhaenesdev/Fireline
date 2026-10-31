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
        CD_Barricade BarricadeConfig { get; }
        CD_Wall WallConfig { get; }
        CD_Barracks BarracksConfig { get; }

        /// <summary>The walls the base starts with: where each stands and which way it faces.</summary>
        int BaseWallCount { get; }
        Vector3 GetBaseWallPoint(int index);
        float GetBaseWallYaw(int index);
        CD_Objective ObjectiveConfig { get; }
        int ObjectivePointCount { get; }
        Vector3 GetObjectivePoint(int index);

        /// <summary>Server only: the objective every client is shown, and how far it is held.</summary>
        void SetObjectiveServer(bool isActive, Vector3 position, float progress);

        /// <summary>Server only: every client hears how the objective ended, and what it paid.</summary>
        void AnnounceObjectiveServer(bool isHeld, int reward);
        int SpawnPointCount { get; }
        Vector3 GetSpawnPoint(int index);

        /// <summary>Server only: the next wave comes in this many seconds from now.</summary>
        void SetCountdownServer(int nextWave, float seconds);

        /// <summary>Server only: a wave is on, with this many zombies still standing.</summary>
        void SetWaveServer(int wave, int zombiesLeft);

        /// <summary>Server only: a structure stands here, of this kind and level, facing this way, with this much health left.</summary>
        void SetStructureServer(int id, int kind, int level, Vector3 position, float yaw, int health);

        void RemoveStructureServer(int id);

        /// <summary>Server only: every client sees the turret fire at this point.</summary>
        void ShowTurretShotServer(int id, Vector3 target);

        /// <summary>Server: tells the buyer it has a structure of this kind to place.</summary>
        void GrantStructureServer(ulong clientId, int kind);

        /// <summary>Client: asks the server to put the local player's structure of this kind here.</summary>
        void RequestPlaceStructure(Vector3 position, int kind);

        /// <summary>Server: shows a hit's damage over the zombie on every client.</summary>
        void ShowZombieHitServer(Vector3 position, int damage);

        /// <summary>Server: a zombie fell; every client shows it toppling in its kind's colour and size.</summary>
        void ShowZombieDeathServer(Vector3 position, float yaw, Color tint, float scale);

        /// <summary>Server: a ring of force bursts out here on every client.</summary>
        void ShowBlastServer(Vector3 position, float radius, Color color);

        /// <summary>Server: a gob of spit flies from here on every client.</summary>
        void ShowSpitServer(Vector3 position, Vector3 velocity, float lifetime);

        /// <summary>Server: the boss whose health every client shows; an empty name hides the bar.</summary>
        void SetBossServer(string name, int health, int maxHealth);

        /// <summary>Server: tells every client the wave was cleared.</summary>
        void AnnounceWaveClearedServer(int wave);

        /// <summary>Client: the structure of this kind waiting to be placed, carried over the local player's head; below zero hides it.</summary>
        void ShowCarried(int kind);
    }
}

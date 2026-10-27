using Unity.Netcode;
using UnityEngine;

namespace Modules.MatchModule.Entities
{
    /// <summary>The match scene's spawn points and what is spawned at them.</summary>
    internal interface IMatchArena
    {
        NetworkObject PlayerPrefab { get; }
        float RespawnDelay { get; }
        int SpawnPointCount { get; }
        Transform GetSpawnPoint(int index);
    }
}

using FlowIoC.ConsoleModule;
using Modules.MatchModule.Entities;
using Unity.Netcode;
using UnityEngine;

namespace Modules.MatchModule.Controllers
{
    /// <summary>Puts a client's player into the arena at the spawn point its id picks. Server only.</summary>
    internal static class PlayerSpawning
    {
        private const float SIDE_STEP = 1.5f;

        public static void Spawn(IMatchArena arena, ulong clientId)
        {
            if (arena == null || arena.PlayerPrefab == null || arena.SpawnPointCount == 0)
            {
                FlowLogger.LogError("PlayerSpawning - the arena has no player prefab or no spawn points.");
                return;
            }

            // More players than points: each lap round the points stands a little further to the side.
            int index = (int) (clientId % (ulong) arena.SpawnPointCount);
            int lap = (int) (clientId / (ulong) arena.SpawnPointCount);
            Transform spawnPoint = arena.GetSpawnPoint(index);
            Vector3 position = spawnPoint.position + spawnPoint.right * (SIDE_STEP * lap);

            NetworkObject player = Object.Instantiate(arena.PlayerPrefab, position, spawnPoint.rotation);
            player.SpawnAsPlayerObject(clientId, true);
        }
    }
}

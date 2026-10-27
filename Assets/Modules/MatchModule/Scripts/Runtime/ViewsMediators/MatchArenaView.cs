using FlowIoC.BaseModule.Injectable.Components;
using FlowIoC.BaseModule.ViewsMediators.View;
using Modules.MatchModule.Entities;
using Unity.Netcode;
using UnityEngine;

namespace Modules.MatchModule.ViewsMediators
{
    /// <summary>The match scene's spawn points and the player prefab spawned at them.</summary>
    [RequireComponent(typeof(ViewInjector))]
    public class MatchArenaView : MonoBehaviour, IView, IMatchArena
    {
        [SerializeField] private NetworkObject _playerPrefab;
        [SerializeField] private Transform[] _spawnPoints;
        [SerializeField] private float _respawnDelay = 2f;

        public bool IsRegistered { get; set; }

        public NetworkObject PlayerPrefab => _playerPrefab;
        public float RespawnDelay => _respawnDelay;
        public int SpawnPointCount => _spawnPoints?.Length ?? 0;
        public Transform GetSpawnPoint(int index) => _spawnPoints[index];
    }
}

using System;
using System.Collections.Generic;
using FlowIoC.BaseModule.Controller;
using FlowIoC.BaseModule.Attributes;
using FlowIoC.BaseModule.Injectable.Attributes;
using FlowIoC.BaseModule.Provider.Coroutine;
using FlowIoC.ConsoleModule;
using Modules.EnemyModule.Data.UnityObjects;
using Modules.EnemyModule.Data.ValueObjects;
using Modules.EnemyModule.Entities;
using Modules.EnemyModule.Models;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Modules.EnemyModule.Controllers
{
    internal class StoreBoardCommand : Command
    {
        [Inject] private IEnemyModel _model { get; set; }

        [SignalParam] private IZombieWaveBoard _board { get; set; }

        public override void Execute() => _model.Board = _board;
    }

    /// <summary>Every process lets Netcode take its zombies from the pool and give them back there.</summary>
    internal class PoolZombiesCommand : Command
    {
        private const string ZOMBIE_POOL_KEY = "Zombie";

        [Inject] private IEnemyModel _model { get; set; }
        [Inject] private FlowIoC.PoolModule.Services.IPoolService _poolService { get; set; }

        public override void Execute()
        {
            GameObject prefab = _model.Board?.Config != null ? _model.Board.Config.ZombiePrefab : null;
            if (prefab == null || NetworkManager.Singleton == null || _model.PooledZombiePrefab == prefab)
                return;

            NetworkManager.Singleton.PrefabHandler.AddHandler(prefab, new NetworkPoolHandler(_poolService, ZOMBIE_POOL_KEY));
            _model.PooledZombiePrefab = prefab;
        }
    }

    /// <summary>Zombies live on the server; on a client the flow ends here.</summary>
    internal class RequireServerCommand : Command
    {
        public override void Execute()
        {
            if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsServer)
                return;

            Retain();
            Stop();
        }
    }

    /// <summary>Waves need their settings and a zombie to spawn; without them the match stays zombie-free.</summary>
    internal class BeginWavesCommand : Command
    {
        [Inject] private IEnemyModel _model { get; set; }

        public override void Execute()
        {
            if (_model.Config == null || _model.Config.ZombiePrefab == null || _model.Config.Waves.Count == 0)
            {
                FlowLogger.LogError("BeginWavesCommand - the ZombieWaves object under EnemySystemRoot has no CD_ZombieWaves with a zombie prefab and at least one wave.");
                Retain();
                Stop();
                return;
            }

            _model.IsRunning = true;
            _model.Wave = 0;
            _model.Spits.Clear();
            _model.DownedClients.Clear();
        }
    }

    internal class StoreDifficultyCommand : Command
    {
        [Inject] private IEnemyModel _model { get; set; }

        [SignalParam] private int _difficulty { get; set; }

        public override void Execute() => _model.Difficulty = _difficulty;
    }

    /// <summary>The match is over: the waves stop where they are and the zombies stand still.</summary>
    internal class StopWavesCommand : Command
    {
        [Inject] private IEnemyModel _model { get; set; }

        public override void Execute()
        {
            _model.IsRunning = false;
            _model.IsTicking = false;
        }
    }

    /// <summary>A revived player is a target again.</summary>
    internal class MarkPlayerUpCommand : Command
    {
        [Inject] private IEnemyModel _model { get; set; }

        [SignalParam] private ulong _clientId { get; set; }

        public override void Execute() => _model.DownedClients.Remove(_clientId);
    }

    /// <summary>A player who went down is no longer anyone's target.</summary>
    internal class MarkPlayerDownCommand : Command
    {
        [Inject] private IEnemyModel _model { get; set; }

        [SignalParam(0)] private ulong _victimClientId { get; set; }

        public override void Execute() => _model.DownedClients.Add(_victimClientId);
    }

    /// <summary>The pause before a wave: a short one before the first, a longer one between the rest.</summary>
    internal class WaitWaveDelayCommand : Command
    {
        [Inject] private IEnemyModel _model { get; set; }
        [Inject] private ICoroutineProvider _coroutineProvider { get; set; }

        public override void Execute()
        {
            Retain();

            if (!_model.IsRunning)
            {
                Stop();
                return;
            }

            float delay = _model.Wave == 0 ? _model.Config.FirstWaveDelay : _model.Config.TimeBetweenWaves;
            _model.Board.SetCountdownServer(_model.Wave + 1, delay);

            IEnemyModel model = _model;
            _coroutineProvider.WaitForSeconds(delay, () =>
            {
                if (model.IsRunning)
                    Release();
                else
                    Stop();
            });
        }
    }

    /// <summary>
    /// The next wave, as the wave table lists it, comes in at the scene's spawn points - those far enough
    /// from every player - or, in a scene with none, on a ring around the players.
    /// </summary>
    internal class SpawnWaveCommand : Command
    {
        private const float POINT_SCATTER = 2f;

        [Inject] private IEnemyModel _model { get; set; }

        public override void Execute()
        {
            List<Transform> players = PlayerTargets.Collect(_model.DownedClients);
            if (players.Count == 0)
                return;

            _model.Wave++;
            CD_ZombieWaves config = _model.Config;
            List<ZombieTypeCVO> wave = new ComposeWaveFunction().Execute(config, _model.Wave, _model.Difficulty);
            List<Vector3> points = FarPoints(players, config.SpawnRadiusMin);

            foreach (ZombieTypeCVO type in wave)
            {
                Vector3 position;
                if (points.Count > 0)
                {
                    Vector2 scatter = Random.insideUnitCircle * POINT_SCATTER;
                    position = points[Random.Range(0, points.Count)] + new Vector3(scatter.x, 0f, scatter.y);
                }
                else
                {
                    Transform anchor = players[Random.Range(0, players.Count)];
                    Vector2 ring = Random.insideUnitCircle.normalized * Random.Range(config.SpawnRadiusMin, config.SpawnRadiusMax);
                    position = anchor.position + new Vector3(ring.x, 0f, ring.y);
                }

                // Taken from the pool through Netcode's handler, on the server and on every client.
                NetworkObject networkObject = config.ZombiePrefab.GetComponent<NetworkObject>()
                    .InstantiateAndSpawn(NetworkManager.Singleton, destroyWithScene: true, position: position, rotation: Quaternion.identity);
                var zombie = networkObject.GetComponent<Zombie>();
                zombie.SetLookServer(type.Tint, type.Scale);

                _model.Zombies[networkObject.NetworkObjectId] = new ZombieRVO
                {
                    Zombie = zombie,
                    Type = type,
                    Health = type.Health,
                    NextAreaTime = Time.time + type.AreaCooldown * 0.5f
                };

                if (type.IsBoss)
                    _model.Board.SetBossServer(type.Name, type.Health, type.Health);
            }

            _model.Board.SetWaveServer(_model.Wave, _model.Zombies.Count);
            FlowLogger.Log($"SpawnWaveCommand - wave {_model.Wave}: {wave.Count} zombies.");
        }

        // Points a player stands right next to are skipped; when every point is close, all of them are used.
        private List<Vector3> FarPoints(List<Transform> players, float minDistance)
        {
            var all = new List<Vector3>();
            var far = new List<Vector3>();
            float minSqr = minDistance * minDistance;

            for (int i = 0; i < _model.Board.SpawnPointCount; i++)
            {
                Vector3 point = _model.Board.GetSpawnPoint(i);
                all.Add(point);

                bool isFar = true;
                foreach (Transform player in players)
                {
                    Vector3 offset = player.position - point;
                    offset.y = 0f;
                    if (offset.sqrMagnitude < minSqr)
                        isFar = false;
                }

                if (isFar)
                    far.Add(point);
            }

            return far.Count > 0 ? far : all;
        }
    }

    /// <summary>The next turn waits until every zombie of this one is down, however long that takes.</summary>
    [LongRetain]
    internal class WaitWaveClearedCommand : Command
    {
        [Inject] private IEnemyModel _model { get; set; }
        [Inject] private ICoroutineProvider _coroutineProvider { get; set; }

        public override void Execute()
        {
            Retain();

            IEnemyModel model = _model;
            _coroutineProvider.WaitUntil(() => !model.IsRunning || model.Zombies.Count == 0, () =>
            {
                if (model.IsRunning)
                    Release();
                else
                    Stop();
            });
        }
    }

    /// <summary>Every client hears that the wave was cleared; the economy pays for it.</summary>
    internal class AnnounceWaveClearedCommand : Command
    {
        [Inject] private IEnemyModel _model { get; set; }

        public override void Execute()
        {
            if (_model.IsRunning)
                _model.Board.AnnounceWaveClearedServer(_model.Wave);
        }
    }

    /// <summary>The living players the zombies can go for.</summary>
    internal static class PlayerTargets
    {
        public static List<Transform> Collect(HashSet<ulong> downedClients)
        {
            var players = new List<Transform>();
            NetworkManager manager = NetworkManager.Singleton;
            if (manager == null)
                return players;

            foreach (NetworkClient client in manager.ConnectedClientsList)
            {
                if (client.PlayerObject != null && client.PlayerObject.IsSpawned && !downedClients.Contains(client.ClientId))
                    players.Add(client.PlayerObject.transform);
            }

            return players;
        }

        public static bool TryNearest(Vector3 from, List<Transform> players, out Transform nearest)
        {
            nearest = null;
            float best = float.MaxValue;

            foreach (Transform player in players)
            {
                float distance = (player.position - from).sqrMagnitude;
                if (distance >= best)
                    continue;

                best = distance;
                nearest = player;
            }

            return nearest != null;
        }
    }
}

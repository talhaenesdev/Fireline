using System;
using System.Collections.Generic;
using FlowIoC.BaseModule.Controller;
using FlowIoC.BaseModule.Attributes;
using FlowIoC.BaseModule.Injectable.Attributes;
using FlowIoC.BaseModule.Provider.Coroutine;
using FlowIoC.ConsoleModule;
using Modules.EnemyModule.Data.UnityObjects;
using Modules.EnemyModule.Entities;
using Modules.EnemyModule.Models;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Modules.EnemyModule.Controllers
{
    internal class StoreConfigCommand : Command
    {
        [Inject] private IEnemyModel _model { get; set; }

        [SignalParam] private CD_ZombieWaves _config { get; set; }

        public override void Execute() => _model.Config = _config;
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
            if (_model.Config == null || _model.Config.ZombiePrefab == null)
            {
                FlowLogger.LogError("BeginWavesCommand - no CD_ZombieWaves with a zombie prefab is filed on EnemySystemRoot.");
                Retain();
                Stop();
                return;
            }

            _model.IsRunning = true;
            _model.Wave = 0;
        }
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
            _coroutineProvider.WaitForSeconds(delay, () => Release());
        }
    }

    /// <summary>The next wave, larger than the last, comes in on a ring around the players.</summary>
    internal class SpawnWaveCommand : Command
    {
        [Inject] private IEnemyModel _model { get; set; }

        public override void Execute()
        {
            List<Transform> players = PlayerTargets.Collect();
            if (players.Count == 0)
                return;

            _model.Wave++;
            CD_ZombieWaves config = _model.Config;
            int count = config.CountForWave(_model.Wave);

            for (int i = 0; i < count; i++)
            {
                Transform anchor = players[Random.Range(0, players.Count)];
                Vector2 ring = Random.insideUnitCircle.normalized * Random.Range(config.SpawnRadiusMin, config.SpawnRadiusMax);
                Vector3 position = anchor.position + new Vector3(ring.x, 0f, ring.y);

                GameObject instance = UnityEngine.Object.Instantiate(config.ZombiePrefab, position, Quaternion.identity);
                var networkObject = instance.GetComponent<NetworkObject>();
                networkObject.Spawn(true);

                _model.Zombies[networkObject.NetworkObjectId] = new ZombieRVO
                {
                    Zombie = instance.GetComponent<Zombie>(),
                    Health = config.Health
                };
            }

            FlowLogger.Log($"SpawnWaveCommand - wave {_model.Wave}: {count} zombies.");
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

    /// <summary>The living players the zombies can go for.</summary>
    internal static class PlayerTargets
    {
        public static List<Transform> Collect()
        {
            var players = new List<Transform>();
            NetworkManager manager = NetworkManager.Singleton;
            if (manager == null)
                return players;

            foreach (NetworkClient client in manager.ConnectedClientsList)
            {
                if (client.PlayerObject != null && client.PlayerObject.IsSpawned)
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

using System.Collections.Generic;
using FlowIoC.BaseModule.Controller;
using FlowIoC.BaseModule.Injectable.Attributes;
using Modules.EnemyModule.Data.UnityObjects;
using Modules.EnemyModule.Entities;
using Modules.EnemyModule.Enums;
using Modules.EnemyModule.Models;
using Modules.EnemyModule.Signals;
using Unity.Netcode;
using UnityEngine;

namespace Modules.EnemyModule.Controllers
{
    /// <summary>Every process lets Netcode take its soldiers from the pool and give them back there.</summary>
    internal class PoolAlliesCommand : Command
    {
        [Inject] private IEnemyModel _model { get; set; }
        [Inject] private FlowIoC.PoolModule.Services.IPoolService _poolService { get; set; }

        public override void Execute()
        {
            CD_Barracks config = _model.Board?.BarracksConfig;
            GameObject prefab = config != null ? config.AllyPrefab : null;
            if (prefab == null || NetworkManager.Singleton == null || _model.PooledAllyPrefab == prefab)
                return;

            NetworkManager.Singleton.PrefabHandler.AddHandler(prefab, new NetworkPoolHandler(_poolService, config.AllyPoolKey));
            _model.PooledAllyPrefab = prefab;
        }
    }

    /// <summary>Server, every frame: each barracks trains a soldier when it is due and short of its number.</summary>
    internal class TrainAlliesCommand : Command
    {
        [Inject] private IEnemyModel _model { get; set; }

        public override void Execute()
        {
            CD_Barracks config = _model.Board?.BarracksConfig;
            if (config == null || config.AllyPrefab == null)
                return;

            foreach (TurretRVO barracks in _model.Turrets.Values)
            {
                if (barracks.Kind != StructureKind.Barracks || Time.time < barracks.NextTrainTime)
                    continue;

                barracks.NextTrainTime = Time.time + config.TrainInterval;

                int count = 0;
                foreach (AllyRVO ally in _model.Allies.Values)
                {
                    if (ally.BarracksId == barracks.Id)
                        count++;
                }

                if (count >= config.MaxAllies)
                    continue;

                // Posts are spread evenly round the barracks, one for each soldier it keeps.
                float angle = (count + 0.5f) / Mathf.Max(1, config.MaxAllies) * Mathf.PI * 2f;
                Vector3 post = barracks.Position + new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * config.PostRadius;

                NetworkObject networkObject = config.AllyPrefab.GetComponent<NetworkObject>()
                    .InstantiateAndSpawn(NetworkManager.Singleton, destroyWithScene: true, position: barracks.Position, rotation: Quaternion.identity);
                var soldier = networkObject.GetComponent<Ally>();
                soldier.SetLookServer(config.AllyTint);

                _model.Allies[networkObject.NetworkObjectId] = new AllyRVO
                {
                    Ally = soldier,
                    BarracksId = barracks.Id,
                    OwnerClientId = barracks.OwnerClientId,
                    Post = post,
                    Health = config.AllyHealth
                };
            }
        }
    }

    /// <summary>
    /// Server, every frame: each soldier goes for the nearest zombie that came close to its post and
    /// shoots it when in range, for the barracks' owner; with none about it walks back to its post.
    /// </summary>
    internal class AlliesFightCommand : Command
    {
        [Inject] private IEnemyModel _model { get; set; }
        [InjectSignal] private EnemySignals _signals { get; set; }

        public override void Execute()
        {
            CD_Barracks config = _model.Board?.BarracksConfig;
            if (config == null || _model.Allies.Count == 0)
                return;

            var shots = new List<(AllyRVO ally, ulong target)>();
            float guardSqr = config.GuardRadius * config.GuardRadius;
            float rangeSqr = config.AllyRange * config.AllyRange;
            float step = config.AllyMoveSpeed * Time.deltaTime;

            foreach (AllyRVO ally in _model.Allies.Values)
            {
                if (ally.Ally == null)
                    continue;

                Vector3 from = ally.Ally.Position;
                ulong targetId = 0;
                Vector3 target = Vector3.zero;
                float best = float.MaxValue;

                foreach (var pair in _model.Zombies)
                {
                    if (pair.Value.Zombie == null)
                        continue;

                    Vector3 position = pair.Value.Zombie.Position;
                    if (Flat(position - ally.Post).sqrMagnitude > guardSqr)
                        continue;

                    float distance = Flat(position - from).sqrMagnitude;
                    if (distance >= best)
                        continue;

                    best = distance;
                    targetId = pair.Key;
                    target = position;
                }

                if (best == float.MaxValue)
                {
                    ally.Ally.StepTowards(ally.Post, step);
                    continue;
                }

                if (best > rangeSqr)
                {
                    ally.Ally.StepTowards(target, step);
                    continue;
                }

                ally.Ally.Face(target);
                if (Time.time < ally.NextFireTime)
                    continue;

                ally.NextFireTime = Time.time + config.AllyFireInterval;
                ally.Ally.ShowShotServer(target);
                shots.Add((ally, targetId));
            }

            // Applied after the search: a shot that fells a zombie takes it out of the list being searched.
            foreach (var shot in shots)
                ZombieDamage.Apply(_model, _signals, shot.target, config.AllyDamage, shot.ally.OwnerClientId, shot.ally.Ally.Position);
        }

        private static Vector3 Flat(Vector3 v) => new(v.x, 0f, v.z);
    }

    /// <summary>What zombies do to soldiers: find the nearest, hurt one, and take a fallen one off the field.</summary>
    internal static class Allies
    {
        public static ulong? Nearest(IEnemyModel model, Vector3 from, List<ulong> fallen, out float distanceSqr)
        {
            ulong? nearest = null;
            distanceSqr = float.MaxValue;
            foreach (var pair in model.Allies)
            {
                if (pair.Value.Ally == null || fallen.Contains(pair.Key))
                    continue;

                Vector3 offset = pair.Value.Ally.Position - from;
                offset.y = 0f;
                if (offset.sqrMagnitude >= distanceSqr)
                    continue;

                distanceSqr = offset.sqrMagnitude;
                nearest = pair.Key;
            }

            return nearest;
        }

        public static void Hurt(IEnemyModel model, ulong id, int damage, List<ulong> fallen)
        {
            if (!model.Allies.TryGetValue(id, out AllyRVO ally) || fallen.Contains(id))
                return;

            ally.Health -= damage;
            ally.Ally.ShowHitServer();
            if (ally.Health <= 0)
                fallen.Add(id);
        }

        public static void RemoveFallen(IEnemyModel model, List<ulong> fallen)
        {
            foreach (ulong id in fallen)
            {
                if (!model.Allies.TryGetValue(id, out AllyRVO ally))
                    continue;

                model.Allies.Remove(id);
                ally.Ally.DespawnServer();
            }
        }
    }
}

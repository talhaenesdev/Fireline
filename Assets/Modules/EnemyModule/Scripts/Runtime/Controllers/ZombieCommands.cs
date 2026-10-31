using System;
using System.Collections.Generic;
using FlowIoC.BaseModule.Controller;
using FlowIoC.BaseModule.Injectable.Attributes;
using FlowIoC.BaseModule.Provider.Update;
using FlowIoC.BaseModule.Signals;
using Modules.EnemyModule.Data.UnityObjects;
using Modules.EnemyModule.Data.ValueObjects;
using Modules.EnemyModule.Enums;
using Modules.EnemyModule.Models;
using Modules.EnemyModule.Signals;
using Modules.MatchModule.Shared.Constants;
using Unity.Netcode;
using UnityEngine;

namespace Modules.EnemyModule.Controllers
{
    /// <summary>Puts the zombies' frame on the update loop, once, and takes it off when ticking stops.</summary>
    internal class StartFrameTickCommand : Command
    {
        [Inject] private IUpdateProvider _updateProvider { get; set; }
        [Inject] private IEnemyModel _model { get; set; }
        [InjectSignal] private EnemyInternalSignals _internalSignals { get; set; }

        public override void Execute()
        {
            _model.IsTicking = true;

            if (_model.IsHooked)
                return;

            _model.IsHooked = true;

            IEnemyModel model = _model;
            IUpdateProvider updateProvider = _updateProvider;
            Signal tick = _internalSignals.Tick;

            Action onFrame = null;
            onFrame = () =>
            {
                if (!model.IsTicking)
                {
                    updateProvider.RemoveUpdate(onFrame);
                    model.IsHooked = false;
                    return;
                }

                tick.Dispatch();
            };

            updateProvider.AddUpdate(onFrame);
        }
    }

    /// <summary>
    /// Every zombie fights the way its kind does: a biter walks at the nearest of the standing players
    /// and turrets and bites, an exploder runs up and blows itself up, a spitter keeps its distance and
    /// spits, a slammer bites and slams the ground around it now and then.
    /// </summary>
    internal class ChaseAndAttackCommand : Command
    {
        private const float SPIT_HEIGHT = 1.2f;

        [Inject] private IEnemyModel _model { get; set; }
        [InjectSignal] private EnemySignals _signals { get; set; }

        public override void Execute()
        {
            if (_model.Zombies.Count == 0)
                return;

            List<Transform> players = PlayerTargets.Collect(_model.DownedClients);
            var fallen = new List<int>();
            var fallenAllies = new List<ulong>();
            var exploded = new List<ulong>();

            foreach (var pair in _model.Zombies)
            {
                ZombieRVO zombie = pair.Value;
                if (zombie.Zombie == null)
                    continue;

                ZombieTypeCVO type = zombie.Type;
                Vector3 from = zombie.Zombie.Position;
                bool hasPlayer = PlayerTargets.TryNearest(from, players, out Transform player);
                TurretRVO turret = NearestTurret(from, fallen);

                // A barricade in the way is broken before anything else.
                TurretRVO blocking = BlockingBarricade(from, fallen);
                if (blocking != null)
                {
                    hasPlayer = false;
                    turret = blocking;
                }

                float playerDistance = hasPlayer ? Flat(player.position - from).sqrMagnitude : float.MaxValue;
                float turretDistance = turret != null ? Flat(turret.Position - from).sqrMagnitude : float.MaxValue;
                float allyDistance = float.MaxValue;
                ulong? ally = null;
                if (blocking == null)
                    ally = Allies.Nearest(_model, from, fallenAllies, out allyDistance);

                if (!hasPlayer && turret == null && ally == null)
                    continue;

                bool goesForAlly = allyDistance < playerDistance && allyDistance < turretDistance;
                bool goesForTurret = !goesForAlly && turretDistance < playerDistance;
                Vector3 target = goesForAlly ? _model.Allies[ally.Value].Ally.Position : goesForTurret ? turret.Position : player.position;
                float distance = Mathf.Sqrt(Mathf.Min(playerDistance, Mathf.Min(turretDistance, allyDistance)));
                float step = type.MoveSpeed * Time.deltaTime;

                switch (type.Behaviour)
                {
                    case ZombieBehaviour.Spitter:
                        Spit(zombie, target, distance, step);
                        continue;

                    case ZombieBehaviour.Exploder:
                        if (distance > type.AttackRange)
                        {
                            zombie.Zombie.StepTowards(target, step);
                            continue;
                        }

                        ZombieArea.Blast(_model, _signals, from, type.AreaRadius, type.AreaDamage, type.Tint, fallen, fallenAllies);
                        exploded.Add(pair.Key);
                        continue;

                    case ZombieBehaviour.Slammer when hasPlayer && playerDistance <= type.AreaRadius * type.AreaRadius && Time.time >= zombie.NextAreaTime:
                        zombie.NextAreaTime = Time.time + type.AreaCooldown;
                        ZombieArea.Blast(_model, _signals, from, type.AreaRadius, type.AreaDamage, type.Tint, fallen, fallenAllies);
                        continue;
                }

                if (distance > type.AttackRange)
                {
                    zombie.Zombie.StepTowards(target, step);
                    continue;
                }

                if (Time.time < zombie.NextAttackTime)
                    continue;

                zombie.NextAttackTime = Time.time + type.AttackCooldown;

                if (goesForAlly)
                {
                    Allies.Hurt(_model, ally.Value, type.Damage, fallenAllies);
                    continue;
                }

                if (!goesForTurret)
                {
                    ulong victim = player.GetComponent<NetworkObject>().OwnerClientId;
                    _signals.Outgoing.PlayerAttacked.Dispatch(victim, type.Damage, MatchConstants.ZOMBIE_ATTACKER_ID);
                    continue;
                }

                ZombieArea.HurtTurret(_model, turret, type.Damage, fallen);
            }

            foreach (int id in fallen)
            {
                _model.Turrets.Remove(id);
                _model.Board.RemoveStructureServer(id);
            }

            Allies.RemoveFallen(_model, fallenAllies);

            // Blown up by its own blast: it falls, but nobody is credited with the kill.
            foreach (ulong id in exploded)
                ZombieDamage.Kill(_model, _signals, id, null);
        }

        // A spitter closes in to its range, then stands and spits, backing off when crowded.
        private void Spit(ZombieRVO zombie, Vector3 target, float distance, float step)
        {
            ZombieTypeCVO type = zombie.Type;
            if (distance > type.PreferredRange)
                zombie.Zombie.StepTowards(target, step);
            else if (distance < type.PreferredRange * 0.5f)
                zombie.Zombie.StepTowards(zombie.Zombie.Position * 2f - target, step * 0.6f);
            else
                zombie.Zombie.Face(target);

            if (distance > type.SpitRange || Time.time < zombie.NextAttackTime)
                return;

            zombie.NextAttackTime = Time.time + type.SpitCooldown;

            Vector3 origin = zombie.Zombie.Position + Vector3.up * SPIT_HEIGHT;
            Vector3 aim = target + Vector3.up * SPIT_HEIGHT - origin;
            aim.y = 0f;
            Vector3 velocity = aim.normalized * type.SpitSpeed;
            float lifetime = type.SpitRange * 1.3f / type.SpitSpeed;

            _model.Spits.Add(new SpitRVO { Position = origin, Velocity = velocity, Damage = type.SpitDamage, TimeLeft = lifetime });
            _model.Board.ShowSpitServer(origin, velocity, lifetime);
        }

        // Barricades and walls stand in the way; each blocks as far as its own config says.
        private TurretRVO BlockingBarricade(Vector3 from, List<int> fallen)
        {
            CD_Barricade barricade = _model.Board.BarricadeConfig;
            CD_Wall wall = _model.Board.WallConfig;

            foreach (TurretRVO structure in _model.Turrets.Values)
            {
                float radius = structure.Kind switch
                {
                    StructureKind.Barricade when barricade != null => barricade.BlockRadius,
                    StructureKind.Wall when wall != null => wall.BlockRadius,
                    _ => 0f
                };

                if (radius > 0f && !fallen.Contains(structure.Id) && Flat(structure.Position - from).sqrMagnitude <= radius * radius)
                    return structure;
            }

            return null;
        }

        private TurretRVO NearestTurret(Vector3 from, List<int> fallen)
        {
            TurretRVO nearest = null;
            float best = float.MaxValue;
            foreach (TurretRVO turret in _model.Turrets.Values)
            {
                if (fallen.Contains(turret.Id))
                    continue;

                float distance = Flat(turret.Position - from).sqrMagnitude;
                if (distance >= best)
                    continue;

                best = distance;
                nearest = turret;
            }

            return nearest;
        }

        private static Vector3 Flat(Vector3 v) => new(v.x, 0f, v.z);
    }

    /// <summary>Server, every frame: spit flies on and hurts the first standing player it reaches.</summary>
    internal class MoveSpitCommand : Command
    {
        private const float HIT_RADIUS = 0.7f;

        [Inject] private IEnemyModel _model { get; set; }
        [InjectSignal] private EnemySignals _signals { get; set; }

        public override void Execute()
        {
            if (_model.Spits.Count == 0)
                return;

            List<Transform> players = PlayerTargets.Collect(_model.DownedClients);
            for (int i = _model.Spits.Count - 1; i >= 0; i--)
            {
                SpitRVO spit = _model.Spits[i];
                spit.Position += spit.Velocity * Time.deltaTime;
                spit.TimeLeft -= Time.deltaTime;

                Transform hit = null;
                foreach (Transform player in players)
                {
                    Vector3 offset = player.position - spit.Position;
                    offset.y = 0f;
                    if (offset.sqrMagnitude <= HIT_RADIUS * HIT_RADIUS)
                        hit = player;
                }

                if (hit != null)
                    _signals.Outgoing.PlayerAttacked.Dispatch(hit.GetComponent<NetworkObject>().OwnerClientId, spit.Damage, MatchConstants.ZOMBIE_ATTACKER_ID);

                if (hit != null || spit.TimeLeft <= 0f)
                    _model.Spits.RemoveAt(i);
            }
        }
    }

    /// <summary>Damage around a point: every standing player and turret inside the radius is hurt.</summary>
    internal static class ZombieArea
    {
        public static void Blast(IEnemyModel model, EnemySignals signals, Vector3 center, float radius, int damage, Color color, List<int> fallen, List<ulong> fallenAllies)
        {
            float radiusSqr = radius * radius;
            foreach (Transform player in PlayerTargets.Collect(model.DownedClients))
            {
                Vector3 offset = player.position - center;
                offset.y = 0f;
                if (offset.sqrMagnitude <= radiusSqr)
                    signals.Outgoing.PlayerAttacked.Dispatch(player.GetComponent<NetworkObject>().OwnerClientId, damage, MatchConstants.ZOMBIE_ATTACKER_ID);
            }

            foreach (TurretRVO turret in model.Turrets.Values)
            {
                Vector3 offset = turret.Position - center;
                offset.y = 0f;
                if (!fallen.Contains(turret.Id) && offset.sqrMagnitude <= radiusSqr)
                    HurtTurret(model, turret, damage, fallen);
            }

            foreach (var pair in model.Allies)
            {
                if (pair.Value.Ally == null)
                    continue;

                Vector3 offset = pair.Value.Ally.Position - center;
                offset.y = 0f;
                if (offset.sqrMagnitude <= radiusSqr)
                    Allies.Hurt(model, pair.Key, damage, fallenAllies);
            }

            model.Board.ShowBlastServer(center, radius, color);
        }

        public static void HurtTurret(IEnemyModel model, TurretRVO turret, int damage, List<int> fallen)
        {
            turret.Health -= damage;
            if (turret.Health > 0)
            {
                Structures.Publish(model, turret);
                return;
            }

            if (!fallen.Contains(turret.Id))
                fallen.Add(turret.Id);
        }
    }

    internal static class ZombieDamage
    {
        /// <summary>
        /// Takes a hit off a zombie, pushed back from where it came and lessened by its front armour if it
        /// came at its front; at zero the zombie falls and the shooter is credited.
        /// </summary>
        public static void Apply(IEnemyModel model, EnemySignals signals, ulong networkObjectId, int damage, ulong shooterClientId, Vector3 from)
        {
            if (!model.Zombies.TryGetValue(networkObjectId, out ZombieRVO zombie))
                return;

            ZombieTypeCVO type = zombie.Type;
            Vector3 toShooter = from - zombie.Zombie.Position;
            toShooter.y = 0f;
            if (type.FrontArmor > 0f && toShooter.sqrMagnitude > 0.01f
                && Vector3.Angle(zombie.Zombie.transform.forward, toShooter) <= type.ArmorAngle * 0.5f)
                damage = Mathf.Max(1, Mathf.RoundToInt(damage * (1f - type.FrontArmor)));

            zombie.Health -= damage;
            zombie.Zombie.TakeHitServer(from, type.HitKnockback);
            model.Board.ShowZombieHitServer(zombie.Zombie.Position, damage);

            if (type.IsBoss)
                model.Board.SetBossServer(type.Name, Mathf.Max(0, zombie.Health), type.Health);

            if (zombie.Health <= 0)
                Kill(model, signals, networkObjectId, shooterClientId);
        }

        /// <summary>The zombie falls: its corpse shows, it drops its loot, and the shooter, if any, is credited.</summary>
        public static void Kill(IEnemyModel model, EnemySignals signals, ulong networkObjectId, ulong? shooterClientId)
        {
            if (!model.Zombies.TryGetValue(networkObjectId, out ZombieRVO zombie))
                return;

            ZombieTypeCVO type = zombie.Type;
            Vector3 fallenAt = zombie.Zombie.Position;
            model.Board.ShowZombieDeathServer(fallenAt, zombie.Zombie.transform.eulerAngles.y, type.Tint, type.Scale);
            model.Zombies.Remove(networkObjectId);
            zombie.Zombie.DespawnServer();
            model.Board.SetWaveServer(model.Wave, model.Zombies.Count);

            if (type.IsBoss)
                model.Board.SetBossServer(string.Empty, 0, 0);

            if (shooterClientId.HasValue)
                signals.Outgoing.ZombieKilled.Dispatch(shooterClientId.Value);

            for (int i = 0; i < Mathf.Max(1, type.LootRolls); i++)
                signals.Outgoing.ZombieDied.Dispatch(fallenAt);
        }
    }

    /// <summary>A round that hit a zombie takes its health down; at zero it falls and the shooter is credited.</summary>
    internal class DamageZombieCommand : Command
    {
        [Inject] private IEnemyModel _model { get; set; }
        [InjectSignal] private EnemySignals _signals { get; set; }

        [SignalParam(0)] private ulong _networkObjectId { get; set; }
        [SignalParam(1)] private ulong _shooterClientId { get; set; }
        [SignalParam] private int _damage { get; set; }

        public override void Execute()
        {
            if (!_model.Zombies.TryGetValue(_networkObjectId, out ZombieRVO zombie))
                return;

            // Pushed back from whoever fired, flashed and bleeding on every screen.
            Vector3 from = zombie.Zombie.Position;
            if (NetworkManager.Singleton.ConnectedClients.TryGetValue(_shooterClientId, out NetworkClient shooter) && shooter.PlayerObject != null)
                from = shooter.PlayerObject.transform.position;

            ZombieDamage.Apply(_model, _signals, _networkObjectId, _damage, _shooterClientId, from);
        }
    }
}

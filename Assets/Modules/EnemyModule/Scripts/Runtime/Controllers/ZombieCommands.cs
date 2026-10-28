using System;
using System.Collections.Generic;
using FlowIoC.BaseModule.Controller;
using FlowIoC.BaseModule.Injectable.Attributes;
using FlowIoC.BaseModule.Provider.Update;
using FlowIoC.BaseModule.Signals;
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
    /// Every zombie walks at whatever is nearest of the standing players and the turrets, and bites
    /// it when close enough and rested. A turret bitten to nothing is gone.
    /// </summary>
    internal class ChaseAndAttackCommand : Command
    {
        [Inject] private IEnemyModel _model { get; set; }
        [InjectSignal] private EnemySignals _signals { get; set; }

        public override void Execute()
        {
            if (_model.Zombies.Count == 0)
                return;

            List<Transform> players = PlayerTargets.Collect(_model.DownedClients);
            float step = _model.Config.MoveSpeed * Time.deltaTime;
            float rangeSqr = _model.Config.AttackRange * _model.Config.AttackRange;

            var fallen = new List<int>();

            foreach (ZombieRVO zombie in _model.Zombies.Values)
            {
                if (zombie.Zombie == null)
                    continue;

                Vector3 from = zombie.Zombie.Position;
                bool hasPlayer = PlayerTargets.TryNearest(from, players, out Transform player);
                TurretRVO turret = NearestTurret(from, fallen);

                float playerDistance = hasPlayer ? Flat(player.position - from).sqrMagnitude : float.MaxValue;
                float turretDistance = turret != null ? Flat(turret.Position - from).sqrMagnitude : float.MaxValue;
                if (!hasPlayer && turret == null)
                    continue;

                bool goesForTurret = turretDistance < playerDistance;
                Vector3 target = goesForTurret ? turret.Position : player.position;

                if (Mathf.Min(playerDistance, turretDistance) > rangeSqr)
                {
                    zombie.Zombie.StepTowards(target, step);
                    continue;
                }

                if (Time.time < zombie.NextAttackTime)
                    continue;

                zombie.NextAttackTime = Time.time + _model.Config.AttackCooldown;

                if (!goesForTurret)
                {
                    ulong victim = player.GetComponent<NetworkObject>().OwnerClientId;
                    _signals.Outgoing.PlayerAttacked.Dispatch(victim, _model.Config.Damage, MatchConstants.ZOMBIE_ATTACKER_ID);
                    continue;
                }

                turret.Health -= _model.Config.Damage;
                if (turret.Health > 0)
                {
                    _model.Board.SetTurretServer(turret.Id, turret.Position, turret.Yaw, turret.Health);
                    continue;
                }

                fallen.Add(turret.Id);
            }

            foreach (int id in fallen)
            {
                _model.Turrets.Remove(id);
                _model.Board.RemoveTurretServer(id);
            }
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

    internal static class ZombieDamage
    {
        /// <summary>Takes a hit off a zombie, pushed back from where it came; at zero the zombie falls and the shooter is credited.</summary>
        public static void Apply(IEnemyModel model, EnemySignals signals, ulong networkObjectId, int damage, ulong shooterClientId, Vector3 from)
        {
            if (!model.Zombies.TryGetValue(networkObjectId, out ZombieRVO zombie))
                return;

            zombie.Health -= damage;
            zombie.Zombie.TakeHitServer(from, model.Config.HitKnockback);

            if (zombie.Health > 0)
                return;

            Vector3 fallenAt = zombie.Zombie.Position;
            model.Zombies.Remove(networkObjectId);
            zombie.Zombie.DespawnServer();
            model.Board.SetWaveServer(model.Wave, model.Zombies.Count);
            signals.Outgoing.ZombieKilled.Dispatch(shooterClientId);
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

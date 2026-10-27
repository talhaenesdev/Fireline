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

    /// <summary>Every zombie walks at the nearest player and bites it when close enough and rested.</summary>
    internal class ChaseAndAttackCommand : Command
    {
        [Inject] private IEnemyModel _model { get; set; }
        [InjectSignal] private EnemySignals _signals { get; set; }

        public override void Execute()
        {
            if (_model.Zombies.Count == 0)
                return;

            List<Transform> players = PlayerTargets.Collect();
            float step = _model.Config.MoveSpeed * Time.deltaTime;
            float rangeSqr = _model.Config.AttackRange * _model.Config.AttackRange;

            foreach (ZombieRVO zombie in _model.Zombies.Values)
            {
                if (zombie.Zombie == null || !PlayerTargets.TryNearest(zombie.Zombie.Position, players, out Transform target))
                    continue;

                Vector3 offset = target.position - zombie.Zombie.Position;
                offset.y = 0f;

                if (offset.sqrMagnitude > rangeSqr)
                {
                    zombie.Zombie.StepTowards(target.position, step);
                    continue;
                }

                if (Time.time < zombie.NextAttackTime)
                    continue;

                zombie.NextAttackTime = Time.time + _model.Config.AttackCooldown;
                ulong victim = target.GetComponent<NetworkObject>().OwnerClientId;
                _signals.Outgoing.PlayerAttacked.Dispatch(victim, _model.Config.Damage, MatchConstants.ZOMBIE_ATTACKER_ID);
            }
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

            zombie.Health -= _damage;

            // Pushed back from whoever fired, flashed and bleeding on every screen.
            Vector3 from = zombie.Zombie.Position;
            if (NetworkManager.Singleton.ConnectedClients.TryGetValue(_shooterClientId, out NetworkClient shooter) && shooter.PlayerObject != null)
                from = shooter.PlayerObject.transform.position;

            zombie.Zombie.TakeHitServer(from, _model.Config.HitKnockback);

            if (zombie.Health > 0)
                return;

            _model.Zombies.Remove(_networkObjectId);
            zombie.Zombie.DespawnServer();
            _signals.Outgoing.ZombieKilled.Dispatch(_shooterClientId);
        }
    }
}

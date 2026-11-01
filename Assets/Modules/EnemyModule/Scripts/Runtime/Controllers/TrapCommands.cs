using System.Collections.Generic;
using FlowIoC.BaseModule.Controller;
using FlowIoC.BaseModule.Injectable.Attributes;
using Modules.EnemyModule.Data.UnityObjects;
using Modules.EnemyModule.Enums;
using Modules.EnemyModule.Models;
using Modules.EnemyModule.Signals;
using UnityEngine;

namespace Modules.EnemyModule.Controllers
{
    /// <summary>
    /// Server, every frame: razor wire slows the zombies on it and hurts them now and then, wearing
    /// down as it does; a mine goes off under the first zombie near it and hurts every one around.
    /// What a trap does is credited to the player who laid it.
    /// </summary>
    internal class TrapsCommand : Command
    {
        [Inject] private IEnemyModel _model { get; set; }
        [InjectSignal] private EnemySignals _signals { get; set; }

        public override void Execute()
        {
            if (_model.Zombies.Count == 0)
                return;

            CD_SpikeTrap spikes = _model.Board.SpikeTrapConfig;
            CD_Mine mine = _model.Board.MineConfig;
            var hits = new List<(ulong zombie, int damage, ulong owner, Vector3 from)>();
            var gone = new List<int>();

            foreach (TurretRVO trap in _model.Turrets.Values)
            {
                if (trap.Kind == StructureKind.SpikeTrap && spikes != null)
                    Wire(trap, spikes, hits, gone);
                else if (trap.Kind == StructureKind.Mine && mine != null)
                    Mine(trap, mine, hits, gone);
            }

            foreach (int id in gone)
            {
                _model.Turrets.Remove(id);
                _model.Board.RemoveStructureServer(id);
            }

            // Applied after the search: a hit that fells a zombie takes it out of the list being searched.
            foreach (var hit in hits)
                ZombieDamage.Apply(_model, _signals, hit.zombie, hit.damage, hit.owner, hit.from);
        }

        private void Wire(TurretRVO trap, CD_SpikeTrap config, List<(ulong, int, ulong, Vector3)> hits, List<int> gone)
        {
            bool bites = Time.time >= trap.NextFireTime;
            int bitten = 0;

            foreach (var pair in _model.Zombies)
            {
                if (pair.Value.Zombie == null || !IsWithin(pair.Value.Zombie.Position, trap.Position, config.Radius))
                    continue;

                // Slowed for as long as it stands on the wire, and a moment after.
                pair.Value.SlowFactor = config.SlowFactor;
                pair.Value.SlowUntil = Time.time + 0.2f;

                if (!bites)
                    continue;

                hits.Add((pair.Key, config.Damage, trap.OwnerClientId, trap.Position));
                bitten++;
            }

            if (!bites || bitten == 0)
                return;

            trap.NextFireTime = Time.time + config.DamageInterval;
            trap.Health -= bitten * config.WearPerHit;
            if (trap.Health <= 0)
                gone.Add(trap.Id);
            else
                Structures.Publish(_model, trap);
        }

        private void Mine(TurretRVO trap, CD_Mine config, List<(ulong, int, ulong, Vector3)> hits, List<int> gone)
        {
            bool isTriggered = false;
            foreach (ZombieRVO zombie in _model.Zombies.Values)
            {
                if (zombie.Zombie != null && IsWithin(zombie.Zombie.Position, trap.Position, config.TriggerRadius))
                {
                    isTriggered = true;
                    break;
                }
            }

            if (!isTriggered)
                return;

            foreach (var pair in _model.Zombies)
            {
                if (pair.Value.Zombie != null && IsWithin(pair.Value.Zombie.Position, trap.Position, config.BlastRadius))
                    hits.Add((pair.Key, config.Damage, trap.OwnerClientId, trap.Position));
            }

            _model.Board.ShowBlastServer(trap.Position, config.BlastRadius, config.BlastColor);
            gone.Add(trap.Id);
        }

        private static bool IsWithin(Vector3 a, Vector3 b, float radius)
        {
            Vector3 offset = a - b;
            offset.y = 0f;
            return offset.sqrMagnitude <= radius * radius;
        }
    }
}

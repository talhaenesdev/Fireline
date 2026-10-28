using FlowIoC.BaseModule.Controller;
using FlowIoC.BaseModule.Injectable.Attributes;
using FlowIoC.ConsoleModule;
using Modules.EnemyModule.Data.UnityObjects;
using Modules.EnemyModule.Models;
using Modules.EnemyModule.Signals;
using Unity.Netcode;
using UnityEngine;

namespace Modules.EnemyModule.Controllers
{
    /// <summary>Server: a bought turret goes down in front of the player who bought it.</summary>
    internal class PlaceTurretCommand : Command
    {
        [Inject] private IEnemyModel _model { get; set; }

        [SignalParam] private ulong _ownerClientId { get; set; }

        public override void Execute()
        {
            CD_Turret config = _model.Board?.TurretConfig;
            if (config == null)
            {
                FlowLogger.LogError("PlaceTurretCommand - the ZombieWaves object has no CD_Turret.");
                return;
            }

            if (!NetworkManager.Singleton.ConnectedClients.TryGetValue(_ownerClientId, out NetworkClient owner) || owner.PlayerObject == null)
                return;

            Transform player = owner.PlayerObject.transform;
            Vector3 forward = new Vector3(player.forward.x, 0f, player.forward.z).normalized;
            Vector3 position = player.position + forward * config.PlaceDistance;
            position.y = 0f;

            var turret = new TurretRVO
            {
                Id = _model.NextTurretId++,
                OwnerClientId = _ownerClientId,
                Position = position,
                Yaw = player.eulerAngles.y,
                Health = config.Health
            };

            _model.Turrets[turret.Id] = turret;
            _model.Board.SetTurretServer(turret.Id, turret.Position, turret.Yaw, turret.Health);
        }
    }

    /// <summary>Server, every frame: each turret turns to the nearest zombie in range and fires when ready, for its owner.</summary>
    internal class TurretsFireCommand : Command
    {
        [Inject] private IEnemyModel _model { get; set; }
        [InjectSignal] private EnemySignals _signals { get; set; }

        public override void Execute()
        {
            CD_Turret config = _model.Board?.TurretConfig;
            if (config == null || _model.Turrets.Count == 0 || _model.Zombies.Count == 0)
                return;

            float rangeSqr = config.Range * config.Range;

            foreach (TurretRVO turret in _model.Turrets.Values)
            {
                if (Time.time < turret.NextFireTime)
                    continue;

                ulong targetId = 0;
                Vector3 targetPosition = Vector3.zero;
                float best = rangeSqr;
                bool found = false;

                foreach (var pair in _model.Zombies)
                {
                    if (pair.Value.Zombie == null)
                        continue;

                    Vector3 offset = pair.Value.Zombie.Position - turret.Position;
                    offset.y = 0f;
                    if (offset.sqrMagnitude > best)
                        continue;

                    best = offset.sqrMagnitude;
                    targetId = pair.Key;
                    targetPosition = pair.Value.Zombie.Position;
                    found = true;
                }

                if (!found)
                    continue;

                Vector3 direction = targetPosition - turret.Position;
                turret.Yaw = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
                turret.NextFireTime = Time.time + config.FireInterval;

                _model.Board.SetTurretServer(turret.Id, turret.Position, turret.Yaw, turret.Health);
                _model.Board.ShowTurretShotServer(turret.Id, targetPosition);
                ZombieDamage.Apply(_model, _signals, targetId, config.Damage, turret.OwnerClientId, turret.Position);
            }
        }
    }
}

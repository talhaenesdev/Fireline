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
    /// <summary>Server: a bought turret is owed to its buyer, who is told to choose where it goes.</summary>
    internal class GrantTurretCommand : Command
    {
        [Inject] private IEnemyModel _model { get; set; }

        [SignalParam] private ulong _clientId { get; set; }

        public override void Execute()
        {
            if (_model.Board?.TurretConfig == null)
            {
                FlowLogger.LogError("GrantTurretCommand - the ZombieWaves object has no CD_Turret.");
                return;
            }

            _model.PendingTurrets[_clientId] = (_model.PendingTurrets.TryGetValue(_clientId, out int owed) ? owed : 0) + 1;
            _model.Board.GrantTurretServer(_clientId);
        }
    }

    /// <summary>Buyer's process: one more turret to place, and placing starts.</summary>
    internal class BeginTurretPlacementCommand : Command
    {
        [Inject] private IEnemyModel _model { get; set; }
        [InjectSignal] private EnemySignals _signals { get; set; }

        public override void Execute()
        {
            _model.LocalTurretsToPlace++;
            _signals.Outgoing.TurretPlacementChanged.Dispatch(true);
        }
    }

    /// <summary>
    /// While a turret is being placed its preview follows the aim, no further from the player than
    /// the turret allows; further than that it would stand at the edge of the reach.
    /// </summary>
    internal class MoveTurretPreviewCommand : Command
    {
        [Inject] private IEnemyModel _model { get; set; }

        [SignalParam] private Vector3 _aim { get; set; }

        public override void Execute()
        {
            if (_model.LocalTurretsToPlace <= 0 || _model.Board?.TurretConfig == null)
                return;

            NetworkObject player = NetworkManager.Singleton.LocalClient?.PlayerObject;
            if (player == null)
                return;

            Vector3 origin = player.transform.position;
            Vector3 offset = _aim - origin;
            offset.y = 0f;
            offset = Vector3.ClampMagnitude(offset, _model.Board.TurretConfig.MaxPlaceDistance);

            _model.PlacementPoint = new Vector3(origin.x + offset.x, 0f, origin.z + offset.z);
            _model.IsPlacementValid = offset.sqrMagnitude > 1f;
            _model.Board.ShowPlacementPreview(true, _model.PlacementPoint, _model.IsPlacementValid);
        }
    }

    /// <summary>The local player puts the turret down where its preview stands; the server checks it.</summary>
    internal class ConfirmTurretPlacementCommand : Command
    {
        [Inject] private IEnemyModel _model { get; set; }
        [InjectSignal] private EnemySignals _signals { get; set; }

        public override void Execute()
        {
            if (_model.LocalTurretsToPlace <= 0 || !_model.IsPlacementValid)
                return;

            _model.Board.RequestPlaceTurret(_model.PlacementPoint);
            _model.LocalTurretsToPlace--;

            if (_model.LocalTurretsToPlace > 0)
                return;

            _model.Board.ShowPlacementPreview(false, Vector3.zero, false);
            _signals.Outgoing.TurretPlacementChanged.Dispatch(false);
        }
    }

    /// <summary>Server: a turret the player is owed goes down where they chose, if it is within their reach.</summary>
    internal class PlaceTurretCommand : Command
    {
        // A little slack for the player having moved while the request travelled.
        private const float REACH_SLACK = 2f;

        [Inject] private IEnemyModel _model { get; set; }

        [SignalParam] private ulong _clientId { get; set; }
        [SignalParam] private Vector3 _position { get; set; }

        public override void Execute()
        {
            CD_Turret config = _model.Board?.TurretConfig;
            if (config == null || !_model.PendingTurrets.TryGetValue(_clientId, out int owed) || owed <= 0)
                return;

            if (!NetworkManager.Singleton.ConnectedClients.TryGetValue(_clientId, out NetworkClient owner) || owner.PlayerObject == null)
                return;

            Vector3 offset = _position - owner.PlayerObject.transform.position;
            offset.y = 0f;
            float reach = config.MaxPlaceDistance + REACH_SLACK;
            if (offset.sqrMagnitude > reach * reach)
                return;

            _model.PendingTurrets[_clientId] = owed - 1;

            var turret = new TurretRVO
            {
                Id = _model.NextTurretId++,
                OwnerClientId = _clientId,
                Position = new Vector3(_position.x, 0f, _position.z),
                Yaw = Mathf.Atan2(offset.x, offset.z) * Mathf.Rad2Deg,
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

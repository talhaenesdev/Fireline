using System.Collections.Generic;
using FlowIoC.BaseModule.Controller;
using FlowIoC.BaseModule.Injectable.Attributes;
using FlowIoC.ConsoleModule;
using Modules.EnemyModule.Data.UnityObjects;
using Modules.EnemyModule.Data.ValueObjects;
using Modules.EnemyModule.Enums;
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

            Structures.Grant(_model, _clientId, StructureKind.Turret);
        }
    }

    /// <summary>Server: a bought barricade is owed to its buyer, who is told to choose where it goes.</summary>
    internal class GrantBarricadeCommand : Command
    {
        [Inject] private IEnemyModel _model { get; set; }

        [SignalParam] private ulong _clientId { get; set; }

        public override void Execute()
        {
            if (_model.Board?.BarricadeConfig == null)
            {
                FlowLogger.LogError("GrantBarricadeCommand - the ZombieWaves object has no CD_Barricade.");
                return;
            }

            Structures.Grant(_model, _clientId, StructureKind.Barricade);
        }
    }

    /// <summary>Server: a bought wall is owed to its buyer, who is told to choose where it goes.</summary>
    internal class GrantWallCommand : Command
    {
        [Inject] private IEnemyModel _model { get; set; }

        [SignalParam] private ulong _clientId { get; set; }

        public override void Execute()
        {
            if (_model.Board?.WallConfig == null)
            {
                FlowLogger.LogError("GrantWallCommand - the ZombieWaves object has no CD_Wall.");
                return;
            }

            Structures.Grant(_model, _clientId, StructureKind.Wall);
        }
    }

    /// <summary>Server: a bought barracks is owed to its buyer, who is told to choose where it goes.</summary>
    internal class GrantBarracksCommand : Command
    {
        [Inject] private IEnemyModel _model { get; set; }

        [SignalParam] private ulong _clientId { get; set; }

        public override void Execute()
        {
            if (_model.Board?.BarracksConfig == null || _model.Board.BarracksConfig.AllyPrefab == null)
            {
                FlowLogger.LogError("GrantBarracksCommand - the ZombieWaves object has no CD_Barracks with a soldier prefab.");
                return;
            }

            Structures.Grant(_model, _clientId, StructureKind.Barracks);
        }
    }

    /// <summary>Server: a bought gate is owed to its buyer, who is told to choose where it goes.</summary>
    internal class GrantGateCommand : Command
    {
        [Inject] private IEnemyModel _model { get; set; }

        [SignalParam] private ulong _clientId { get; set; }

        public override void Execute()
        {
            if (_model.Board?.GateConfig == null)
            {
                FlowLogger.LogError("GrantGateCommand - the ZombieWaves object has no CD_Gate.");
                return;
            }

            Structures.Grant(_model, _clientId, StructureKind.Gate);
        }
    }

    /// <summary>Server: a bought spike trap is owed to its buyer, who is told to choose where it goes.</summary>
    internal class GrantSpikeTrapCommand : Command
    {
        [Inject] private IEnemyModel _model { get; set; }

        [SignalParam] private ulong _clientId { get; set; }

        public override void Execute()
        {
            if (_model.Board?.SpikeTrapConfig == null)
            {
                FlowLogger.LogError("GrantSpikeTrapCommand - the ZombieWaves object has no CD_SpikeTrap.");
                return;
            }

            Structures.Grant(_model, _clientId, StructureKind.SpikeTrap);
        }
    }

    /// <summary>Server: a bought mine is owed to its buyer, who is told to choose where it goes.</summary>
    internal class GrantMineCommand : Command
    {
        [Inject] private IEnemyModel _model { get; set; }

        [SignalParam] private ulong _clientId { get; set; }

        public override void Execute()
        {
            if (_model.Board?.MineConfig == null)
            {
                FlowLogger.LogError("GrantMineCommand - the ZombieWaves object has no CD_Mine.");
                return;
            }

            Structures.Grant(_model, _clientId, StructureKind.Mine);
        }
    }

    /// <summary>Server: every wall, the base's own too, goes up a tier and keeps the damage it had taken.</summary>
    internal class ReinforceWallsCommand : Command
    {
        [Inject] private IEnemyModel _model { get; set; }

        public override void Execute()
        {
            CD_Wall config = _model.Board?.WallConfig;
            if (config == null || _model.WallTier >= config.TierCount - 1)
                return;

            _model.WallTier++;
            foreach (TurretRVO wall in _model.Turrets.Values)
            {
                if (wall.Kind != StructureKind.Wall)
                    continue;

                int damage = config.Tier(wall.Level).Health - wall.Health;
                wall.Level = _model.WallTier;
                wall.Health = Mathf.Max(1, config.Tier(wall.Level).Health - damage);
                Structures.Publish(_model, wall);
            }
        }
    }

    /// <summary>Server: a bought repair station is owed to its buyer, who is told to choose where it goes.</summary>
    internal class GrantRepairStationCommand : Command
    {
        [Inject] private IEnemyModel _model { get; set; }

        [SignalParam] private ulong _clientId { get; set; }

        public override void Execute()
        {
            if (_model.Board?.RepairStationConfig == null)
            {
                FlowLogger.LogError("GrantRepairStationCommand - the ZombieWaves object has no CD_RepairStation.");
                return;
            }

            Structures.Grant(_model, _clientId, StructureKind.RepairStation);
        }
    }

    /// <summary>Server, as the waves begin: the headquarters goes up at the base's centre, if the map has one.</summary>
    internal class RaiseHeadquartersCommand : Command
    {
        [Inject] private IEnemyModel _model { get; set; }

        public override void Execute()
        {
            CD_Headquarters config = _model.Board.HeadquartersConfig;
            if (config == null)
                return;

            Vector3 center = _model.Board.BaseCenter;
            var headquarters = new TurretRVO
            {
                Id = _model.NextTurretId++,
                Kind = StructureKind.Headquarters,
                OwnerClientId = NetworkManager.ServerClientId,
                Position = new Vector3(center.x, 0f, center.z),
                Health = config.Health
            };

            _model.Turrets[headquarters.Id] = headquarters;
            Structures.Publish(_model, headquarters);
        }
    }

    /// <summary>Server, every frame: each repair station mends the damaged structures around it, for free.</summary>
    internal class RepairStationsMendCommand : Command
    {
        [Inject] private IEnemyModel _model { get; set; }

        public override void Execute()
        {
            CD_RepairStation config = _model.Board?.RepairStationConfig;
            if (config == null)
                return;

            float radiusSqr = config.Radius * config.Radius;
            float mend = config.HealthPerSecond * Time.deltaTime;

            foreach (TurretRVO station in _model.Turrets.Values)
            {
                if (station.Kind != StructureKind.RepairStation)
                    continue;

                foreach (TurretRVO structure in _model.Turrets.Values)
                {
                    Vector3 offset = structure.Position - station.Position;
                    offset.y = 0f;
                    int max = Structures.MaxHealth(_model, structure.Kind, structure.Level);
                    if (structure.Health >= max || offset.sqrMagnitude > radiusSqr)
                        continue;

                    // Whole points only; the rest is carried to the next frame, so a slow station still mends.
                    structure.MendCarry += mend;
                    int whole = Mathf.FloorToInt(structure.MendCarry);
                    if (whole <= 0)
                        continue;

                    structure.MendCarry -= whole;
                    structure.Health = Mathf.Min(max, structure.Health + whole);
                    Structures.Publish(_model, structure);
                }
            }
        }
    }

    /// <summary>Server, as the waves begin: the base's own walls go up, owned by nobody.</summary>
    internal class RaiseBaseWallsCommand : Command
    {
        [Inject] private IEnemyModel _model { get; set; }

        public override void Execute()
        {
            if (_model.Board.WallConfig == null)
                return;

            foreach ((Vector3 point, float yaw) in new LayOutBaseFunction().Execute(_model.Board))
            {
                var wall = new TurretRVO
                {
                    Id = _model.NextTurretId++,
                    Kind = StructureKind.Wall,
                    OwnerClientId = NetworkManager.ServerClientId,
                    Position = new Vector3(point.x, 0f, point.z),
                    Yaw = yaw,
                    Health = _model.Board.WallConfig.Health
                };

                _model.Turrets[wall.Id] = wall;
                Structures.Publish(_model, wall);
            }
        }
    }

    /// <summary>Buyer's process: one more structure to place; the first waiting one is carried over the player's head.</summary>
    internal class BeginStructurePlacementCommand : Command
    {
        [Inject] private IEnemyModel _model { get; set; }
        [InjectSignal] private EnemySignals _signals { get; set; }

        [SignalParam] private int _kind { get; set; }

        public override void Execute()
        {
            _model.LocalStructuresToPlace.Add((StructureKind) _kind);
            _model.Board.ShowCarried((int) _model.LocalStructuresToPlace[0]);
            _signals.Outgoing.TurretPlacementChanged.Dispatch(true);
        }
    }

    /// <summary>The local player puts the carried structure down where they stand; the server checks it.</summary>
    internal class ConfirmStructurePlacementCommand : Command
    {
        [Inject] private IEnemyModel _model { get; set; }
        [InjectSignal] private EnemySignals _signals { get; set; }

        public override void Execute()
        {
            NetworkObject player = NetworkManager.Singleton.LocalClient?.PlayerObject;
            if (_model.LocalStructuresToPlace.Count == 0 || player == null)
                return;

            StructureKind kind = _model.LocalStructuresToPlace[0];
            _model.LocalStructuresToPlace.RemoveAt(0);

            Vector3 position = player.transform.position;
            _model.Board.RequestPlaceStructure(new Vector3(position.x, 0f, position.z), (int) kind);

            if (_model.LocalStructuresToPlace.Count > 0)
            {
                _model.Board.ShowCarried((int) _model.LocalStructuresToPlace[0]);
                return;
            }

            _model.Board.ShowCarried(-1);
            _signals.Outgoing.TurretPlacementChanged.Dispatch(false);
        }
    }

    /// <summary>Server: a structure the player is owed goes down where they chose, if it is within their reach.</summary>
    internal class PlaceStructureCommand : Command
    {
        // A little slack for the player having moved while the request travelled.
        private const float REACH_SLACK = 2f;

        [Inject] private IEnemyModel _model { get; set; }

        [SignalParam] private ulong _clientId { get; set; }
        [SignalParam] private Vector3 _position { get; set; }
        [SignalParam] private int _kind { get; set; }

        public override void Execute()
        {
            var kind = (StructureKind) _kind;
            if (!_model.PendingStructures.TryGetValue(_clientId, out List<StructureKind> owed) || !owed.Contains(kind))
                return;

            if (!NetworkManager.Singleton.ConnectedClients.TryGetValue(_clientId, out NetworkClient owner) || owner.PlayerObject == null)
                return;

            float maxDistance = Structures.MaxPlaceDistance(_model, kind);
            Transform player = owner.PlayerObject.transform;
            Vector3 offset = _position - player.position;
            offset.y = 0f;
            float reach = maxDistance + REACH_SLACK;
            if (offset.sqrMagnitude > reach * reach)
                return;

            owed.Remove(kind);

            var structure = new TurretRVO
            {
                Id = _model.NextTurretId++,
                Kind = kind,
                Level = kind == StructureKind.Wall ? _model.WallTier : 0,
                OwnerClientId = _clientId,
                Position = new Vector3(_position.x, 0f, _position.z),
                Yaw = player.eulerAngles.y,
                Health = Structures.MaxHealth(_model, kind, kind == StructureKind.Wall ? _model.WallTier : 0),
                NextTrainTime = Time.time
            };

            _model.Turrets[structure.Id] = structure;
            Structures.Publish(_model, structure);
        }
    }

    /// <summary>Server: every turret of the buyer goes up a level, as far as there are levels, and is mended to its new health.</summary>
    internal class UpgradeTurretsCommand : Command
    {
        [Inject] private IEnemyModel _model { get; set; }

        [SignalParam] private ulong _clientId { get; set; }

        public override void Execute()
        {
            CD_Turret config = _model.Board?.TurretConfig;
            if (config == null)
                return;

            foreach (TurretRVO turret in _model.Turrets.Values)
            {
                if (turret.Kind != StructureKind.Turret || turret.OwnerClientId != _clientId || turret.Level >= config.LevelCount - 1)
                    continue;

                turret.Level++;
                turret.Health = config.Level(turret.Level).Health;
                Structures.Publish(_model, turret);
            }
        }
    }

    /// <summary>Server, every frame: each turret turns to the nearest zombie in its level's range and fires when ready, for its owner.</summary>
    internal class TurretsFireCommand : Command
    {
        [Inject] private IEnemyModel _model { get; set; }
        [InjectSignal] private EnemySignals _signals { get; set; }

        public override void Execute()
        {
            CD_Turret config = _model.Board?.TurretConfig;
            if (config == null || _model.Turrets.Count == 0 || _model.Zombies.Count == 0)
                return;

            var shots = new List<(TurretRVO turret, ulong target, Vector3 at, int damage)>();

            foreach (TurretRVO turret in _model.Turrets.Values)
            {
                if (turret.Kind != StructureKind.Turret || Time.time < turret.NextFireTime)
                    continue;

                TurretLevelCVO level = config.Level(turret.Level);
                ulong targetId = 0;
                Vector3 targetPosition = Vector3.zero;
                float best = level.Range * level.Range;
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
                turret.NextFireTime = Time.time + level.FireInterval;
                shots.Add((turret, targetId, targetPosition, level.Damage));
            }

            // Applied after the search: a shot that fells a zombie takes it out of the list being searched.
            foreach (var shot in shots)
            {
                Structures.Publish(_model, shot.turret);
                _model.Board.ShowTurretShotServer(shot.turret.Id, shot.at);
                ZombieDamage.Apply(_model, _signals, shot.target, shot.damage, shot.turret.OwnerClientId, shot.turret.Position);
            }
        }
    }

    /// <summary>What every structure command shares: owing one, its health, and showing it.</summary>
    internal static class Structures
    {
        public static void Grant(IEnemyModel model, ulong clientId, StructureKind kind)
        {
            if (!model.PendingStructures.TryGetValue(clientId, out List<StructureKind> owed))
                model.PendingStructures[clientId] = owed = new List<StructureKind>();

            owed.Add(kind);
            model.Board.GrantStructureServer(clientId, (int) kind);
        }

        public static int MaxHealth(IEnemyModel model, StructureKind kind, int level) => kind switch
        {
            StructureKind.Barricade => model.Board.BarricadeConfig.Health,
            StructureKind.Wall => model.Board.WallConfig.Tier(level).Health,
            StructureKind.SpikeTrap => model.Board.SpikeTrapConfig.Health,
            StructureKind.Mine => model.Board.MineConfig.Health,
            StructureKind.Barracks => model.Board.BarracksConfig.Health,
            StructureKind.Gate => model.Board.GateConfig.Health,
            StructureKind.RepairStation => model.Board.RepairStationConfig.Health,
            StructureKind.Headquarters => model.Board.HeadquartersConfig.Health,
            _ => model.Board.TurretConfig.Level(level).Health
        };

        public static float MaxPlaceDistance(IEnemyModel model, StructureKind kind) => kind switch
        {
            StructureKind.Barricade => model.Board.BarricadeConfig.MaxPlaceDistance,
            StructureKind.Wall => model.Board.WallConfig.MaxPlaceDistance,
            StructureKind.Barracks => model.Board.BarracksConfig.MaxPlaceDistance,
            StructureKind.Gate => model.Board.GateConfig.MaxPlaceDistance,
            StructureKind.RepairStation => model.Board.RepairStationConfig.MaxPlaceDistance,
            StructureKind.SpikeTrap => model.Board.SpikeTrapConfig.MaxPlaceDistance,
            StructureKind.Mine => model.Board.MineConfig.MaxPlaceDistance,
            _ => model.Board.TurretConfig.MaxPlaceDistance
        };

        public static void Publish(IEnemyModel model, TurretRVO structure) =>
            model.Board.SetStructureServer(structure.Id, (int) structure.Kind, structure.Level, structure.Position, structure.Yaw, structure.Health);
    }
}

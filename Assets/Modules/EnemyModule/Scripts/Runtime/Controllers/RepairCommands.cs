using FlowIoC.BaseModule.Controller;
using FlowIoC.BaseModule.Injectable.Attributes;
using Modules.EnemyModule.Data.UnityObjects;
using Modules.EnemyModule.Models;
using Modules.EnemyModule.Signals;
using Unity.Netcode;
using UnityEngine;

namespace Modules.EnemyModule.Controllers
{
    /// <summary>The local player asks the server to mend what they stand by.</summary>
    internal class RequestRepairCommand : Command
    {
        [Inject] private IEnemyModel _model { get; set; }

        public override void Execute()
        {
            if (_model.Board != null && _model.Board.RepairConfig != null)
                _model.Board.RequestRepair();
        }
    }

    /// <summary>
    /// Server: the damaged structure nearest the player, within reach, is priced for one press of
    /// mending; the wallet that pays is another module's, so the quote goes out.
    /// </summary>
    internal class QuoteRepairCommand : Command
    {
        [Inject] private IEnemyModel _model { get; set; }
        [InjectSignal] private EnemySignals _signals { get; set; }

        [SignalParam] private ulong _clientId { get; set; }

        public override void Execute()
        {
            CD_Repair config = _model.Board.RepairConfig;
            if (config == null || !NetworkManager.Singleton.ConnectedClients.TryGetValue(_clientId, out NetworkClient client) || client.PlayerObject == null)
                return;

            Vector3 from = client.PlayerObject.transform.position;
            TurretRVO nearest = null;
            int missing = 0;
            float best = config.Range * config.Range;

            foreach (TurretRVO structure in _model.Turrets.Values)
            {
                int gap = Structures.MaxHealth(_model, structure.Kind, structure.Level) - structure.Health;
                Vector3 offset = structure.Position - from;
                offset.y = 0f;
                if (gap <= 0 || offset.sqrMagnitude > best)
                    continue;

                best = offset.sqrMagnitude;
                nearest = structure;
                missing = gap;
            }

            if (nearest == null)
                return;

            int health = Mathf.Min(missing, config.HealthPerPress);
            int cost = Mathf.Max(1, Mathf.CeilToInt(health * config.CostPerHealth));
            _signals.Outgoing.RepairQuoted.Dispatch(_clientId, nearest.Id, health, cost);
        }
    }

    /// <summary>Server: a paid repair gives the structure its health back, as far as its most.</summary>
    internal class RepairStructureCommand : Command
    {
        [Inject] private IEnemyModel _model { get; set; }

        [SignalParam(0)] private int _structureId { get; set; }
        [SignalParam(1)] private int _health { get; set; }

        public override void Execute()
        {
            if (!_model.Turrets.TryGetValue(_structureId, out TurretRVO structure))
                return;

            int max = Structures.MaxHealth(_model, structure.Kind, structure.Level);
            structure.Health = Mathf.Min(max, structure.Health + _health);
            Structures.Publish(_model, structure);
        }
    }
}

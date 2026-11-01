using FlowIoC.BaseModule.Controller;
using FlowIoC.BaseModule.Injectable.Attributes;
using Modules.EconomyModule.Models;
using Modules.EconomyModule.Signals;
using UnityEngine;

namespace Modules.EconomyModule.Controllers
{
    /// <summary>
    /// Server: the player pays for a repair; one who cannot pay it all pays what they have and gets
    /// that share of the health.
    /// </summary>
    internal class ChargeRepairCommand : Command
    {
        [Inject] private IEconomyModel _model { get; set; }
        [InjectSignal] private EconomySignals _signals { get; set; }

        [SignalParam] private ulong _clientId { get; set; }
        [SignalParam(0)] private int _structureId { get; set; }
        [SignalParam(1)] private int _health { get; set; }
        [SignalParam(2)] private int _cost { get; set; }

        public override void Execute()
        {
            int money = EconomyWallets.MoneyOf(_model, _clientId);
            if (money <= 0 || _cost <= 0 || _health <= 0)
                return;

            int paid = Mathf.Min(money, _cost);
            int health = paid >= _cost ? _health : Mathf.FloorToInt((float) _health * paid / _cost);
            if (health <= 0)
                return;

            _model.Money[_clientId] = money - paid;
            EconomyWallets.Publish(_model, _clientId);
            _signals.Outgoing.RepairPaid.Dispatch(_structureId, health);
        }
    }
}

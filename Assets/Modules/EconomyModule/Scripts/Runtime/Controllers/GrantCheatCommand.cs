using FlowIoC.BaseModule.Controller;
using FlowIoC.BaseModule.Injectable.Attributes;
using Modules.EconomyModule.Models;

namespace Modules.EconomyModule.Controllers
{
    /// <summary>Server, test builds only: the player who pressed the cheat key gets money and materials.</summary>
    internal class GrantCheatCommand : Command
    {
        private const int MONEY = 1000;
        private const int MATERIALS = 100;

        [Inject] private IEconomyModel _model { get; set; }

        [SignalParam] private ulong _clientId { get; set; }

        public override void Execute()
        {
            if (_model.Board == null)
                return;

            _model.Money[_clientId] = EconomyWallets.MoneyOf(_model, _clientId) + MONEY;
            _model.Materials[_clientId] = EconomyWallets.MaterialsOf(_model, _clientId) + MATERIALS;
            EconomyWallets.Publish(_model, _clientId);
        }
    }
}

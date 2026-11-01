using FlowIoC.BaseModule.Controller;
using FlowIoC.BaseModule.Injectable.Attributes;
using FlowIoC.PoolModule.Services;
using Modules.EconomyModule.Entities;

namespace Modules.EconomyModule.Controllers
{
    /// <summary>A registered board is handed the pool it draws from.</summary>
    internal class ConfigureBoardPoolCommand : Command
    {
        [Inject] private IPoolService _poolService { get; set; }

        [SignalParam] private IEconomyBoard _board { get; set; }

        public override void Execute() => _board.Configure(_poolService);
    }
}

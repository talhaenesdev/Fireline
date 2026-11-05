using FlowIoC.BaseModule.Controller;
using FlowIoC.BaseModule.Injectable.Attributes;
using FlowIoC.PoolModule.Services;
using Modules.EnemyModule.Entities;
using Modules.EnemyModule.Models;

namespace Modules.EnemyModule.Controllers
{
    /// <summary>A registered board is handed the pool it draws from.</summary>
    internal class ConfigureBoardPoolCommand : Command
    {
        [Inject] private IPoolService _poolService { get; set; }

        [SignalParam] private IZombieWaveBoard _board { get; set; }

        public override void Execute() => _board.Configure(_poolService);
    }

    /// <summary>A board that registered before the module launched is handed the pool now.</summary>
    internal class ConfigureStoredBoardPoolCommand : Command
    {
        [Inject] private IPoolService _poolService { get; set; }
        [Inject] private IEnemyModel _model { get; set; }

        public override void Execute() => _model.Board?.Configure(_poolService);
    }

    internal class MarkLaunchedCommand : Command
    {
        [Inject] private IEnemyModel _model { get; set; }

        public override void Execute() => _model.IsLaunched = true;
    }

    /// <summary>Before the module has launched the pool groups are not registered; the flow ends here.</summary>
    internal class RequireLaunchedCommand : Command
    {
        [Inject] private IEnemyModel _model { get; set; }

        public override void Execute()
        {
            if (_model.IsLaunched)
                return;

            Retain();
            Stop();
        }
    }
}

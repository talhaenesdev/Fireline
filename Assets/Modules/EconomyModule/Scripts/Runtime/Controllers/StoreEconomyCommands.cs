using FlowIoC.BaseModule.Controller;
using FlowIoC.BaseModule.Injectable.Attributes;
using Modules.EconomyModule.Entities;
using Modules.EconomyModule.Models;

namespace Modules.EconomyModule.Controllers
{
    internal class StoreBoardCommand : Command
    {
        [Inject] private IEconomyModel _model { get; set; }

        [SignalParam] private IEconomyBoard _board { get; set; }

        public override void Execute() => _model.Board = _board;
    }

    internal class StoreZoneCommand : Command
    {
        [Inject] private IEconomyModel _model { get; set; }

        [SignalParam] private IEconomyZone _zone { get; set; }

        public override void Execute() => _model.Zones.Add(_zone);
    }

    /// <summary>The steps after this one are the server's to take; on a client the flow ends here.</summary>
    internal class RequireServerCommand : Command
    {
        [Inject] private IEconomyModel _model { get; set; }

        public override void Execute()
        {
            if (_model.Board != null && _model.Board.IsSpawned && _model.Board.IsServer)
                return;

            Retain();
            Stop();
        }
    }
}

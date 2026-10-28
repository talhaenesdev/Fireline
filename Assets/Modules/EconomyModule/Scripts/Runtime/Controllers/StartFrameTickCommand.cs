using System;
using FlowIoC.BaseModule.Controller;
using FlowIoC.BaseModule.Injectable.Attributes;
using FlowIoC.BaseModule.Provider.Update;
using FlowIoC.BaseModule.Signals;
using Modules.EconomyModule.Models;
using Modules.EconomyModule.Signals;

namespace Modules.EconomyModule.Controllers
{
    /// <summary>Puts the economy's frame on the update loop, once, and takes it off when ticking stops.</summary>
    internal class StartFrameTickCommand : Command
    {
        [Inject] private IUpdateProvider _updateProvider { get; set; }
        [Inject] private IEconomyModel _model { get; set; }
        [InjectSignal] private EconomyInternalSignals _internalSignals { get; set; }

        public override void Execute()
        {
            _model.IsTicking = true;

            if (_model.IsHooked)
                return;

            _model.IsHooked = true;

            IEconomyModel model = _model;
            IUpdateProvider updateProvider = _updateProvider;
            Signal tick = _internalSignals.Tick;

            Action onFrame = null;
            onFrame = () =>
            {
                if (!model.IsTicking)
                {
                    updateProvider.RemoveUpdate(onFrame);
                    model.IsHooked = false;
                    return;
                }

                tick.Dispatch();
            };

            updateProvider.AddUpdate(onFrame);
        }
    }
}

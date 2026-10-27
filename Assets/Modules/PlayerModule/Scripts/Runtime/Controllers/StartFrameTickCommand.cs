using System;
using FlowIoC.BaseModule.Controller;
using FlowIoC.BaseModule.Injectable.Attributes;
using FlowIoC.BaseModule.Provider.Update;
using FlowIoC.BaseModule.Signals;
using Modules.PlayerModule.Models;
using Modules.PlayerModule.Signals;

namespace Modules.PlayerModule.Controllers
{
    /// <summary>Puts the local player's frame on the update loop, once, and takes it off when ticking stops.</summary>
    internal class StartFrameTickCommand : Command
    {
        [Inject] private IUpdateProvider _updateProvider { get; set; }
        [Inject] private IPlayerModel _model { get; set; }
        [InjectSignal] private PlayerInternalSignals _internalSignals { get; set; }

        public override void Execute()
        {
            _model.IsTicking = true;

            // A callback still on the frame carries on; a second one would tick twice a frame.
            if (_model.IsHooked)
                return;

            _model.IsHooked = true;

            // Taken into locals: this instance goes back to the pool when Execute returns.
            IPlayerModel model = _model;
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

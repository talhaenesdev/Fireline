using System;
using FlowIoC.BaseModule.Controller;
using FlowIoC.BaseModule.Injectable.Attributes;
using FlowIoC.BaseModule.Provider.Update;
using FlowIoC.BaseModule.Signals;
using Modules.WeaponModule.Models;
using Modules.WeaponModule.Signals;

namespace Modules.WeaponModule.Controllers
{
    /// <summary>Puts the local weapon's frame on the update loop, once, and takes it off when ticking stops.</summary>
    internal class StartFrameTickCommand : Command
    {
        [Inject] private IUpdateProvider _updateProvider { get; set; }
        [Inject] private IWeaponModel _model { get; set; }
        [InjectSignal] private WeaponInternalSignals _internalSignals { get; set; }

        public override void Execute()
        {
            _model.IsTicking = true;

            if (_model.IsHooked)
                return;

            _model.IsHooked = true;

            IWeaponModel model = _model;
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

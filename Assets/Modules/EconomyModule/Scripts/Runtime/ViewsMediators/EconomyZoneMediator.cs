using FlowIoC.BaseModule.Injectable.Attributes;
using FlowIoC.BaseModule.ViewsMediators.Mediator;
using Modules.EconomyModule.Signals;

namespace Modules.EconomyModule.ViewsMediators
{
    public class EconomyZoneMediator : IMediator
    {
        [Inject] private EconomyZoneView _view { get; set; }
        [InjectSignal] private EconomyInternalSignals _internalSignals { get; set; }

        public void OnRegister() => _internalSignals.ZoneRegistered.Dispatch(_view);

        public void OnRemove()
        {
        }
    }
}

using FlowIoC.BaseModule.Injectable.Attributes;
using FlowIoC.BaseModule.ViewsMediators.Mediator;
using Modules.MatchModule.Signals;

namespace Modules.MatchModule.ViewsMediators
{
    public class MatchArenaMediator : IMediator
    {
        [Inject] private MatchArenaView _view { get; set; }
        [InjectSignal] private MatchInternalSignals _internalSignals { get; set; }

        public void OnRegister() => _internalSignals.ArenaRegistered.Dispatch(_view);

        public void OnRemove()
        {
        }
    }
}

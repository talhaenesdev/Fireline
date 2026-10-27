using FlowIoC.BaseModule.Injectable.Attributes;
using FlowIoC.BaseModule.ViewsMediators.Mediator;
using Modules.EnemyModule.Signals;

namespace Modules.EnemyModule.ViewsMediators
{
    public class ZombieWavesMediator : IMediator
    {
        [Inject] private ZombieWavesView _view { get; set; }
        [InjectSignal] private EnemyInternalSignals _internalSignals { get; set; }

        public void OnRegister() => _internalSignals.ConfigRegistered.Dispatch(_view.Config);

        public void OnRemove()
        {
        }
    }
}

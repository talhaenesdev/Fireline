using FlowIoC.BaseModule.Injectable.Attributes;
using FlowIoC.BaseModule.ViewsMediators.Mediator;
using Modules.EnemyModule.Signals;

namespace Modules.EnemyModule.ViewsMediators
{
    public class ZombieWavesMediator : IMediator
    {
        [Inject] private ZombieWavesView _view { get; set; }
        [InjectSignal] private EnemySignals _signals { get; set; }
        [InjectSignal] private EnemyInternalSignals _internalSignals { get; set; }

        public void OnRegister()
        {
            _view.StateChanged += OnStateChanged;
            _view.TurretGranted += OnTurretGranted;
            _view.TurretPlaceRequested += OnTurretPlaceRequested;
            _internalSignals.BoardRegistered.Dispatch(_view);
        }

        public void OnRemove()
        {
            _view.StateChanged -= OnStateChanged;
            _view.TurretGranted -= OnTurretGranted;
            _view.TurretPlaceRequested -= OnTurretPlaceRequested;
        }

        private void OnTurretGranted() => _internalSignals.TurretGranted.Dispatch();

        private void OnTurretPlaceRequested(ulong clientId, UnityEngine.Vector3 position) =>
            _internalSignals.TurretPlaceRequested.Dispatch(clientId, position);

        private void OnStateChanged(int wave, float secondsUntilWave, int zombiesLeft) =>
            _signals.Outgoing.WaveStateChanged.Dispatch(wave, secondsUntilWave, zombiesLeft);
    }
}

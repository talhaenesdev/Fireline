using FlowIoC.BaseModule.Injectable.Attributes;
using FlowIoC.BaseModule.ViewsMediators.Mediator;
using FlowIoC.PoolModule.Services;
using Modules.EnemyModule.Signals;

namespace Modules.EnemyModule.ViewsMediators
{
    public class ZombieWavesMediator : IMediator
    {
        [Inject] private ZombieWavesView _view { get; set; }
        [Inject] private IPoolService _poolService { get; set; }
        [InjectSignal] private EnemySignals _signals { get; set; }
        [InjectSignal] private EnemyInternalSignals _internalSignals { get; set; }

        public void OnRegister()
        {
            _view.StateChanged += OnStateChanged;
            _view.StructureGranted += OnStructureGranted;
            _view.StructurePlaceRequested += OnStructurePlaceRequested;
            _view.WaveCleared += OnWaveCleared;
            _view.BossChanged += OnBossChanged;
            _view.ObjectiveChanged += OnObjectiveChanged;
            _view.ObjectiveEnded += OnObjectiveEnded;
            _view.Configure(_poolService);
            _internalSignals.BoardRegistered.Dispatch(_view);
        }

        public void OnRemove()
        {
            _view.StateChanged -= OnStateChanged;
            _view.StructureGranted -= OnStructureGranted;
            _view.StructurePlaceRequested -= OnStructurePlaceRequested;
            _view.WaveCleared -= OnWaveCleared;
            _view.BossChanged -= OnBossChanged;
            _view.ObjectiveChanged -= OnObjectiveChanged;
            _view.ObjectiveEnded -= OnObjectiveEnded;
        }

        private void OnObjectiveChanged(string title, bool isActive, float progress) =>
            _signals.Outgoing.ObjectiveChanged.Dispatch(title, isActive, progress);

        private void OnObjectiveEnded(bool isHeld, int reward) => _signals.Outgoing.ObjectiveEnded.Dispatch(isHeld, reward);

        private void OnBossChanged(string name, int health, int maxHealth) =>
            _signals.Outgoing.BossStateChanged.Dispatch(name, health, maxHealth);

        private void OnWaveCleared(int wave) => _signals.Outgoing.WaveCleared.Dispatch(wave);

        private void OnStructureGranted(int kind) => _internalSignals.StructureGranted.Dispatch(kind);

        private void OnStructurePlaceRequested(ulong clientId, UnityEngine.Vector3 position, int kind) =>
            _internalSignals.StructurePlaceRequested.Dispatch(clientId, position, kind);

        private void OnStateChanged(int wave, float secondsUntilWave, int zombiesLeft) =>
            _signals.Outgoing.WaveStateChanged.Dispatch(wave, secondsUntilWave, zombiesLeft);
    }
}

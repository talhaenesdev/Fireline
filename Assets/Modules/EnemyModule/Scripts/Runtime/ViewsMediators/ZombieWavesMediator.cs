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
            _view.StructureGranted += OnStructureGranted;
            _view.StructurePlaceRequested += OnStructurePlaceRequested;
            _view.RepairRequested += OnRepairRequested;
            _view.AllyOrderRequested += OnAllyOrderRequested;
            _view.AllyOrderAnnounced += OnAllyOrderAnnounced;
            _view.WaveCleared += OnWaveCleared;
            _view.BossChanged += OnBossChanged;
            _view.ObjectiveChanged += OnObjectiveChanged;
            _view.ObjectiveEnded += OnObjectiveEnded;
            _view.ZombieHitShown += OnZombieHitShown;
            _view.ZombieDeathShown += OnZombieDeathShown;
            _view.BlastShown += OnBlastShown;
            _view.StructureFellShown += OnStructureFellShown;
            _internalSignals.PoolRequested.Dispatch(_view);
            _internalSignals.BoardRegistered.Dispatch(_view);
        }

        public void OnRemove()
        {
            _view.StateChanged -= OnStateChanged;
            _view.StructureGranted -= OnStructureGranted;
            _view.StructurePlaceRequested -= OnStructurePlaceRequested;
            _view.RepairRequested -= OnRepairRequested;
            _view.AllyOrderRequested -= OnAllyOrderRequested;
            _view.AllyOrderAnnounced -= OnAllyOrderAnnounced;
            _view.WaveCleared -= OnWaveCleared;
            _view.BossChanged -= OnBossChanged;
            _view.ObjectiveChanged -= OnObjectiveChanged;
            _view.ObjectiveEnded -= OnObjectiveEnded;
            _view.ZombieHitShown -= OnZombieHitShown;
            _view.ZombieDeathShown -= OnZombieDeathShown;
            _view.BlastShown -= OnBlastShown;
            _view.StructureFellShown -= OnStructureFellShown;
        }

        private void OnZombieHitShown(UnityEngine.Vector3 position) => _internalSignals.ZombieHitShown.Dispatch(position);

        private void OnZombieDeathShown(UnityEngine.Vector3 position) => _internalSignals.ZombieDeathShown.Dispatch(position);

        private void OnBlastShown(UnityEngine.Vector3 position) => _internalSignals.BlastShown.Dispatch(position);

        private void OnStructureFellShown(UnityEngine.Vector3 position) => _internalSignals.StructureFellShown.Dispatch(position);

        private void OnObjectiveChanged(string title, bool isActive, float progress) =>
            _signals.Outgoing.ObjectiveChanged.Dispatch(title, isActive, progress);

        private void OnObjectiveEnded(bool isHeld, int reward) => _signals.Outgoing.ObjectiveEnded.Dispatch(isHeld, reward);

        private void OnBossChanged(string name, int health, int maxHealth) =>
            _signals.Outgoing.BossStateChanged.Dispatch(name, health, maxHealth);

        private void OnWaveCleared(int wave)
        {
            _internalSignals.WaveClearedShown.Dispatch();
            _signals.Outgoing.WaveCleared.Dispatch(wave);
        }

        private void OnAllyOrderRequested(ulong clientId) => _internalSignals.AllyOrderRequested.Dispatch(clientId);

        private void OnAllyOrderAnnounced(bool isFollowing) => _signals.Outgoing.AllyOrderChanged.Dispatch(isFollowing);

        private void OnRepairRequested(ulong clientId) => _internalSignals.RepairRequested.Dispatch(clientId);

        private void OnStructureGranted(int kind) => _internalSignals.StructureGranted.Dispatch(kind);

        private void OnStructurePlaceRequested(ulong clientId, UnityEngine.Vector3 position, int kind) =>
            _internalSignals.StructurePlaceRequested.Dispatch(clientId, position, kind);

        private void OnStateChanged(int wave, float secondsUntilWave, int zombiesLeft) =>
            _signals.Outgoing.WaveStateChanged.Dispatch(wave, secondsUntilWave, zombiesLeft);
    }
}

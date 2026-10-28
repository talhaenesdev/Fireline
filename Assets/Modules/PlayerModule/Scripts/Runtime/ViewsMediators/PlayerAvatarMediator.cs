using FlowIoC.BaseModule.Injectable.Attributes;
using FlowIoC.BaseModule.ViewsMediators.Mediator;
using Modules.PlayerModule.Signals;

namespace Modules.PlayerModule.ViewsMediators
{
    public class PlayerAvatarMediator : IMediator
    {
        [Inject] private PlayerAvatarView _view { get; set; }
        [InjectSignal] private PlayerInternalSignals _internalSignals { get; set; }

        private bool _announced;

        public void OnRegister()
        {
            _view.Spawned += OnSpawned;
            _view.Despawned += OnDespawned;
            _view.NameRequested += OnNameRequested;
            _view.NameChanged += OnNameChanged;
            _view.HealthChanged += OnHealthChanged;
            _view.DeathChanged += OnDeathChanged;
            _view.ShieldChanged += OnShieldChanged;
            _view.ReviveRequested += OnReviveRequested;

            // Netcode spawns the avatar before the injector's Start registers it, so the spawn has
            // usually happened by the time the Mediator exists.
            if (_view.IsSpawned)
                OnSpawned();
        }

        public void OnRemove()
        {
            _view.Spawned -= OnSpawned;
            _view.Despawned -= OnDespawned;
            _view.NameRequested -= OnNameRequested;
            _view.NameChanged -= OnNameChanged;
            _view.HealthChanged -= OnHealthChanged;
            _view.DeathChanged -= OnDeathChanged;
            _view.ShieldChanged -= OnShieldChanged;
            _view.ReviveRequested -= OnReviveRequested;

            OnDespawned();
        }

        private void OnSpawned()
        {
            if (_announced)
                return;

            _announced = true;
            _internalSignals.AvatarSpawned.Dispatch(_view);
            _internalSignals.HealthChanged.Dispatch(_view.ClientId, _view.Health, _view.MaxHealth);
            _internalSignals.ShieldChanged.Dispatch(_view.ClientId, _view.Shield);
        }

        private void OnDespawned()
        {
            if (!_announced)
                return;

            _announced = false;
            _internalSignals.AvatarDespawned.Dispatch(_view);
        }

        private void OnNameRequested(string playerName) => _internalSignals.NameRequested.Dispatch(_view.ClientId, playerName);

        private void OnNameChanged(string playerName) => _internalSignals.NameChanged.Dispatch(_view.ClientId, playerName);

        private void OnHealthChanged(float health, float maxHealth) =>
            _internalSignals.HealthChanged.Dispatch(_view.ClientId, health, maxHealth);

        private void OnShieldChanged(float shield) => _internalSignals.ShieldChanged.Dispatch(_view.ClientId, shield);

        private void OnReviveRequested(ulong targetClientId) => _internalSignals.ReviveRequested.Dispatch(_view.ClientId, targetClientId);

        // How death looks is the same whoever decided it, so the avatar shows it at once.
        private void OnDeathChanged(bool isDead)
        {
            _view.ShowDeath(isDead);
            _internalSignals.DeathChanged.Dispatch(_view.ClientId, isDead);
        }
    }
}

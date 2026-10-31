using FlowIoC.BaseModule.Injectable.Attributes;
using FlowIoC.BaseModule.ViewsMediators.Mediator;
using Modules.WeaponModule.Signals;
using UnityEngine;

namespace Modules.WeaponModule.ViewsMediators
{
    public class WeaponMediator : IMediator
    {
        [Inject] private WeaponView _view { get; set; }
        [InjectSignal] private WeaponInternalSignals _internalSignals { get; set; }

        private bool _announced;

        public void OnRegister()
        {
            _view.Spawned += OnSpawned;
            _view.Despawned += OnDespawned;
            _view.ServerFireRequested += OnServerFireRequested;
            _view.RemoteFired += OnRemoteFired;
            _view.WeaponChanged += OnWeaponChanged;

            // Netcode spawns the player before the injector's Start registers this view.
            if (_view.IsSpawned)
                OnSpawned();
        }

        public void OnRemove()
        {
            _view.Spawned -= OnSpawned;
            _view.Despawned -= OnDespawned;
            _view.ServerFireRequested -= OnServerFireRequested;
            _view.RemoteFired -= OnRemoteFired;
            _view.WeaponChanged -= OnWeaponChanged;
            _internalSignals.ShowLocalFire.RemoveListener(_view.ShowFire);

            OnDespawned();
        }

        private void OnSpawned()
        {
            if (_announced)
                return;

            _announced = true;

            if (_view.IsOwner)
                _internalSignals.ShowLocalFire.AddListener(_view.ShowFire);

            _internalSignals.WeaponSpawned.Dispatch(_view);
        }

        private void OnDespawned()
        {
            if (!_announced)
                return;

            _announced = false;
            _internalSignals.WeaponDespawned.Dispatch(_view);
        }

        private void OnServerFireRequested(Vector3 position, Vector3 direction) =>
            _internalSignals.ServerFireRequested.Dispatch(_view, position, direction);

        private void OnWeaponChanged() => _internalSignals.WeaponChanged.Dispatch(_view);

        private void OnRemoteFired(Vector3 position)
        {
            _view.ShowFire();
            _internalSignals.RemoteFired.Dispatch(position);
        }
    }
}

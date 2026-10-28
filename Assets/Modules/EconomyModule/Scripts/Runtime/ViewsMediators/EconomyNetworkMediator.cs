using FlowIoC.BaseModule.Injectable.Attributes;
using FlowIoC.BaseModule.ViewsMediators.Mediator;
using Modules.EconomyModule.Signals;

namespace Modules.EconomyModule.ViewsMediators
{
    public class EconomyNetworkMediator : IMediator
    {
        [Inject] private EconomyNetworkView _view { get; set; }
        [InjectSignal] private EconomyInternalSignals _internalSignals { get; set; }

        public void OnRegister()
        {
            _view.Spawned += OnSpawned;
            _view.WalletsChanged += OnWalletsChanged;
            _view.BuyRequested += OnBuyRequested;
            _view.Purchased += OnPurchased;

            if (_view.IsSpawned)
            {
                OnSpawned();
                OnWalletsChanged();
            }
        }

        public void OnRemove()
        {
            _view.Spawned -= OnSpawned;
            _view.WalletsChanged -= OnWalletsChanged;
            _view.BuyRequested -= OnBuyRequested;
            _view.Purchased -= OnPurchased;
        }

        private void OnSpawned() => _internalSignals.BoardRegistered.Dispatch(_view);

        private void OnWalletsChanged() => _internalSignals.WalletsChanged.Dispatch();

        private void OnBuyRequested(ulong clientId, int item) => _internalSignals.BuyRequested.Dispatch(clientId, item);

        private void OnPurchased(int item) => _internalSignals.Purchased.Dispatch(item);
    }
}

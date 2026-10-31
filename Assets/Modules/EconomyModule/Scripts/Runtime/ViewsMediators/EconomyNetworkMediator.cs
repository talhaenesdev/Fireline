using FlowIoC.BaseModule.Injectable.Attributes;
using FlowIoC.BaseModule.ViewsMediators.Mediator;
using FlowIoC.PoolModule.Services;
using Modules.EconomyModule.Signals;

namespace Modules.EconomyModule.ViewsMediators
{
    public class EconomyNetworkMediator : IMediator
    {
        [Inject] private EconomyNetworkView _view { get; set; }
        [Inject] private IPoolService _poolService { get; set; }
        [InjectSignal] private EconomyInternalSignals _internalSignals { get; set; }

        public void OnRegister()
        {
            _view.Spawned += OnSpawned;
            _view.WalletsChanged += OnWalletsChanged;
            _view.BuyRequested += OnBuyRequested;
            _view.Purchased += OnPurchased;
            _view.DoorsChanged += OnDoorsChanged;
            _view.LevelReported += OnLevelReported;
            _view.Configure(_poolService);

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
            _view.DoorsChanged -= OnDoorsChanged;
            _view.LevelReported -= OnLevelReported;
        }

        private void OnDoorsChanged() => _internalSignals.DoorsChanged.Dispatch();

        private void OnLevelReported(ulong clientId, int level) => _internalSignals.LevelReported.Dispatch(clientId, level);

        private void OnSpawned() => _internalSignals.BoardRegistered.Dispatch(_view);

        private void OnWalletsChanged() => _internalSignals.WalletsChanged.Dispatch();

        private void OnBuyRequested(ulong clientId, int zone, int item) => _internalSignals.BuyRequested.Dispatch(clientId, zone, item);

        private void OnPurchased(int zone, int item) => _internalSignals.Purchased.Dispatch(zone, item);
    }
}

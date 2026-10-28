using FlowIoC.BaseModule.Injectable.Attributes;
using FlowIoC.BaseModule.ViewsMediators.Mediator;
using FlowIoC.ScreenModule.Enums;
using FlowIoC.ScreenModule.Extensions;
using FlowIoC.ScreenModule.ViewsMediators.Screen;
using Modules.EconomyModule.ShopScreenModule.Signals;

namespace Modules.EconomyModule.ShopScreenModule.ViewsMediators
{
    public class ShopScreenMediator : IMediator
    {
        [Inject] private ShopScreenView _view { get; set; }
        [InjectSignal] private ShopScreenSignals _signals { get; set; }

        public void OnRegister()
        {
            _view.ShowCompleted += OnScreenShown;
            _view.HideCompleted += OnScreenHidden;
        }

        public void OnRemove()
        {
            _view.ShowCompleted -= OnScreenShown;
            _view.HideCompleted -= OnScreenHidden;
        }

        private void OnScreenShown(IScreenBody screen)
        {
            _view.BuyClicked += OnBuyClicked;
            _signals.Incoming.ApplyMoney.AddListener(_view.ShowMoney);
        }

        private void OnScreenHidden(IScreenBody screen)
        {
            _view.BuyClicked -= OnBuyClicked;
            _signals.Incoming.ApplyMoney.RemoveListener(_view.ShowMoney);
        }

        private void OnBuyClicked(int item)
        {
            if (_view.Data.HasState(ScreenState.AvailableToSendSignal))
                _signals.Outgoing.BuyClicked.Dispatch(item);
        }
    }
}

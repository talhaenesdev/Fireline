using FlowIoC.ScreenModule.Data;
using FlowIoC.ScreenModule.Enums;
using FlowIoC.ScreenModule.RootsContexts;
using Modules.EconomyModule.ShopScreenModule.Controllers;
using Modules.EconomyModule.ShopScreenModule.Models;
using Modules.EconomyModule.ShopScreenModule.Signals;
using Modules.EconomyModule.ShopScreenModule.ViewsMediators;

namespace Modules.EconomyModule.ShopScreenModule.RootsContexts
{
    public class ShopScreenContext : ScreenSubContext<ShopScreenView, ShopScreenMediator>
    {
        private ShopScreenSignals _signals;

        protected override ScreenCVO Screen => new()
        {
            ManagerId = 0,
            Layer = 2,
            Tag = ScreenTag.Default,
            Load = ScreenLoadCVO.Addressable("ShopScreen"),
            HasShowAnimation = false,
            HasHideAnimation = false,
        };

        public override void SignalBindings()
        {
            base.SignalBindings();
            _signals = InjectionBinderCrossContext.Bind<ShopScreenSignals>();
        }

        public override void InjectionBindings()
        {
            base.InjectionBindings();
            InjectionBinder.Bind<IShopScreenModel, ShopScreenModel>();
        }

        public override void CommandBindings()
        {
            base.CommandBindings();

            CommandBinder.Bind(_signals.Incoming.Open)
                .ToSequence<RememberItemsCommand>()
                .ToSequence<OpenShopScreenCommand>();

            CommandBinder.Bind(_signals.Incoming.Close).ToSequence<CloseShopScreenCommand>();
            CommandBinder.Bind(_signals.Incoming.ApplyMoney).ToSequence<RememberMoneyCommand>();
        }
    }
}

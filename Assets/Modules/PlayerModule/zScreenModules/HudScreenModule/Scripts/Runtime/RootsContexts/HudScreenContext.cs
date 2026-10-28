using FlowIoC.ScreenModule.Data;
using FlowIoC.ScreenModule.Enums;
using FlowIoC.ScreenModule.RootsContexts;
using Modules.PlayerModule.HudScreenModule.Controllers;
using Modules.PlayerModule.HudScreenModule.Models;
using Modules.PlayerModule.HudScreenModule.Signals;
using Modules.PlayerModule.HudScreenModule.ViewsMediators;

namespace Modules.PlayerModule.HudScreenModule.RootsContexts
{
    public class HudScreenContext : ScreenSubContext<HudScreenView, HudScreenMediator>
    {
        private HudScreenSignals _signals;

        protected override ScreenCVO Screen => new()
        {
            ManagerId = 0,
            Layer = 0,
            Tag = ScreenTag.Default,
            Load = ScreenLoadCVO.Addressable("HudScreen"),
            HasShowAnimation = false,
            HasHideAnimation = false,
        };

        public override void SignalBindings()
        {
            base.SignalBindings();
            _signals = InjectionBinderCrossContext.Bind<HudScreenSignals>();
        }

        public override void InjectionBindings()
        {
            base.InjectionBindings();
            InjectionBinder.Bind<IHudScreenModel, HudScreenModel>();
        }

        public override void CommandBindings()
        {
            base.CommandBindings();
            CommandBinder.Bind(_signals.Incoming.Open).ToSequence<OpenHudScreenCommand>();
            CommandBinder.Bind(_signals.Incoming.ApplyHealth).ToSequence<RememberHealthCommand>();
            CommandBinder.Bind(_signals.Incoming.ApplyAmmo).ToSequence<RememberAmmoCommand>();
            CommandBinder.Bind(_signals.Incoming.ApplyWave).ToSequence<RememberWaveCommand>();
            CommandBinder.Bind(_signals.Incoming.ApplyShield).ToSequence<RememberShieldCommand>();
            CommandBinder.Bind(_signals.Incoming.ApplyWallet).ToSequence<RememberWalletCommand>();
        }
    }
}

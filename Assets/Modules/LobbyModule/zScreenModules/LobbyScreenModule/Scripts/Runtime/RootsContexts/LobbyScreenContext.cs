using FlowIoC.ScreenModule.Data;
using FlowIoC.ScreenModule.Enums;
using FlowIoC.ScreenModule.RootsContexts;
using Modules.LobbyModule.LobbyScreenModule.Controllers;
using Modules.LobbyModule.LobbyScreenModule.Models;
using Modules.LobbyModule.LobbyScreenModule.Signals;
using Modules.LobbyModule.LobbyScreenModule.ViewsMediators;

namespace Modules.LobbyModule.LobbyScreenModule.RootsContexts
{
    public class LobbyScreenContext : ScreenSubContext<LobbyScreenView, LobbyScreenMediator>
    {
        private LobbyScreenSignals _signals;

        protected override ScreenCVO Screen => new()
        {
            ManagerId = 0,
            Layer = 1,
            Tag = ScreenTag.Default,
            Load = ScreenLoadCVO.Addressable("LobbyScreen"),
            HasShowAnimation = false,
            HasHideAnimation = false,
        };

        public override void SignalBindings()
        {
            base.SignalBindings();
            _signals = InjectionBinderCrossContext.Bind<LobbyScreenSignals>();
        }

        public override void InjectionBindings()
        {
            base.InjectionBindings();
            InjectionBinder.Bind<ILobbyScreenModel, LobbyScreenModel>();
        }

        public override void CommandBindings()
        {
            base.CommandBindings();
            CommandBinder.Bind(_signals.Incoming.Toggle).ToSequence<ToggleLobbyScreenCommand>();
            CommandBinder.Bind(_signals.Incoming.ApplyState).ToSequence<RememberLobbyStateCommand>();
        }
    }
}

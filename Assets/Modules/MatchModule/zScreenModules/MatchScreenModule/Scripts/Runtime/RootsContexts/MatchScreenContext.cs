using FlowIoC.ScreenModule.Data;
using FlowIoC.ScreenModule.Enums;
using FlowIoC.ScreenModule.RootsContexts;
using Modules.MatchModule.MatchScreenModule.Controllers;
using Modules.MatchModule.MatchScreenModule.Models;
using Modules.MatchModule.MatchScreenModule.Signals;
using Modules.MatchModule.MatchScreenModule.ViewsMediators;

namespace Modules.MatchModule.MatchScreenModule.RootsContexts
{
    public class MatchScreenContext : ScreenSubContext<MatchScreenView, MatchScreenMediator>
    {
        private MatchScreenSignals _signals;

        protected override ScreenCVO Screen => new()
        {
            ManagerId = 0,
            Layer = 1,
            Tag = ScreenTag.Default,
            Load = ScreenLoadCVO.Addressable("MatchScreen"),
            HasShowAnimation = false,
            HasHideAnimation = false,
        };

        public override void SignalBindings()
        {
            base.SignalBindings();
            _signals = InjectionBinderCrossContext.Bind<MatchScreenSignals>();
        }

        public override void InjectionBindings()
        {
            base.InjectionBindings();
            InjectionBinder.Bind<IMatchScreenModel, MatchScreenModel>();
        }

        public override void CommandBindings()
        {
            base.CommandBindings();
            CommandBinder.Bind(_signals.Incoming.Open).ToSequence<OpenMatchScreenCommand>();
            CommandBinder.Bind(_signals.Incoming.ApplyScores).ToSequence<RememberScoresCommand>();
        }
    }
}

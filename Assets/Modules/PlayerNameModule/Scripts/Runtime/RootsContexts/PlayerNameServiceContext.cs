using FlowIoC.BaseModule.Contexts;
using Modules.PlayerNameModule.Services;

namespace Modules.PlayerNameModule.RootsContexts
{

    public class PlayerNameServiceContext : Context
    {
        public override void SignalBindings()
        {
            base.SignalBindings();
        }

        public override void InjectionBindings()
        {
            base.InjectionBindings();
            InjectionBinderCrossContext.Bind<IPlayerNameService, PlayerNameService>();
        }

        public override void MediationBindings()
        {
            base.MediationBindings();
        }

        public override void CommandBindings()
        {
            base.CommandBindings();
        }

        public override void Setup()
        {
            base.Setup();
        }

        public override void Launch()
        {
            base.Launch();
        }
    }
}

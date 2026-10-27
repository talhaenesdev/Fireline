using FlowIoC.BaseModule.Contexts;
using Modules.NetworkSessionModule.Services;
using Modules.NetworkSessionModule.Signals;

namespace Modules.NetworkSessionModule.RootsContexts
{
    public class NetworkSessionServiceContext : Context
    {
        private INetworkSessionService _service;

        public override void SignalBindings()
        {
            base.SignalBindings();
            InjectionBinderCrossContext.Bind<NetworkSessionSignals>();
        }

        public override void InjectionBindings()
        {
            base.InjectionBindings();

            // The one type other modules reference directly, which is what makes this a Service.
            _service = InjectionBinderCrossContext.Bind<INetworkSessionService, NetworkSessionService>();
        }

        public override void Setup()
        {
            base.Setup();
            _service.HookNetworkManager();
        }

        public override void DestroyContext()
        {
            _service?.UnhookNetworkManager();
            base.DestroyContext();
        }
    }
}

using FlowIoC.BaseModule.Contexts;
using Modules.LocalizationModule.Services;

namespace Modules.LocalizationModule.RootsContexts
{
    public class LocalizationServiceContext : Context
    {
        public override void InjectionBindings()
        {
            base.InjectionBindings();
            InjectionBinderCrossContext.Bind<ILocalizationService, LocalizationService>();
        }
    }
}

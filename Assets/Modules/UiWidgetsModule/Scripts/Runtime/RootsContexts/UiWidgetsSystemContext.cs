using FlowIoC.BaseModule.Contexts;
using Modules.AudioModule.Services;
using Modules.AudioModule.Shared;
using Modules.UiWidgetsModule.Signals;
using Modules.UiWidgetsModule.ViewsMediators;

namespace Modules.UiWidgetsModule.RootsContexts
{
    public class UiWidgetsSystemContext : Context
    {
        private UiWidgetsInternalSignals _internalSignals;

        public override void SignalBindings()
        {
            base.SignalBindings();
            _internalSignals = InjectionBinder.Bind<UiWidgetsInternalSignals>();
        }

        public override void MediationBindings()
        {
            base.MediationBindings();
            MediationBinder.Bind<UiButtonAudioView>().To<UiButtonAudioMediator>();
        }

        public override void CommandBindings()
        {
            base.CommandBindings();
            CommandBinder.Bind(_internalSignals.ButtonHovered).ToSequence<IAudioService.Commands.Play>(AudioKey.UiWidgets.ButtonHover);
            CommandBinder.Bind(_internalSignals.ButtonClicked).ToSequence<IAudioService.Commands.Play>(AudioKey.UiWidgets.ButtonClick);
        }
    }
}

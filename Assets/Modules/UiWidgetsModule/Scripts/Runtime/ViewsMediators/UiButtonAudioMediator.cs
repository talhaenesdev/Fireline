using FlowIoC.BaseModule.Injectable.Attributes;
using FlowIoC.BaseModule.ViewsMediators.Mediator;
using Modules.UiWidgetsModule.Signals;

namespace Modules.UiWidgetsModule.ViewsMediators
{
    public class UiButtonAudioMediator : IMediator
    {
        [Inject] private UiButtonAudioView _view { get; set; }
        [InjectSignal] private UiWidgetsInternalSignals _internalSignals { get; set; }

        public void OnRegister()
        {
            _view.Hovered += _internalSignals.ButtonHovered.Dispatch;
            _view.Clicked += _internalSignals.ButtonClicked.Dispatch;
        }

        public void OnRemove()
        {
            _view.Hovered -= _internalSignals.ButtonHovered.Dispatch;
            _view.Clicked -= _internalSignals.ButtonClicked.Dispatch;
        }
    }
}

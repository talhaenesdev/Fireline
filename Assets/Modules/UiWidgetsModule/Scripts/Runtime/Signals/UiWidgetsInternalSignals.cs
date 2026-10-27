using FlowIoC.BaseModule.Signals;

namespace Modules.UiWidgetsModule.Signals
{
    internal class UiWidgetsInternalSignals : ISignalHolder
    {
        public Signal ButtonHovered = new();
        public Signal ButtonClicked = new();
    }
}

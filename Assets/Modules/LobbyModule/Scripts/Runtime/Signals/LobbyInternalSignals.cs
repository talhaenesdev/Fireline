using FlowIoC.BaseModule.Signals;

namespace Modules.LobbyModule.Signals
{
    /// <summary>What the module says to its own commands.</summary>
    internal class LobbyInternalSignals : ISignalHolder
    {
        public Signal Launch = new();
    }
}

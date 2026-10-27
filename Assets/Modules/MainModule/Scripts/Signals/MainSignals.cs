using FlowIoC.BaseModule.Signals;

namespace Modules.MainModule.Signals
{
    public class MainSignals : ISignalHolder
    {
        public MainSignalsIncoming Incoming = new();
        public MainSignalsOutgoing Outgoing = new();

        public class MainSignalsIncoming
        {
            public Signal RetryBoot = new();
            public Signal Quit = new();
        }

        public class MainSignalsOutgoing
        {
            public Signal BootStarted = new();
            public Signal Started = new();
        }
    }
}

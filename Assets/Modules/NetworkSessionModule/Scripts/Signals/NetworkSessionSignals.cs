using FlowIoC.BaseModule.Signals;
using Modules.NetworkSessionModule.Shared.Data.ValueObjects;

namespace Modules.NetworkSessionModule.Signals
{
    public class NetworkSessionSignals : ISignalHolder
    {
        public NetworkSessionSignalsIncoming Incoming = new();
        public NetworkSessionSignalsOutgoing Outgoing = new();
    }

    public class NetworkSessionSignalsIncoming
    {
    }

    public class NetworkSessionSignalsOutgoing
    {
        /// <summary>The session's players, code or ready flags changed, or the session ended.</summary>
        public Signal<SessionStateVO> SessionChanged = new();

        /// <summary>This process became the server.</summary>
        public Signal ServerStarted = new();

        /// <summary>A client connected to the server this process runs. Dispatched on the server only.</summary>
        public Signal<ulong> ClientConnected = new();

        /// <summary>A client left the server this process runs. Dispatched on the server only.</summary>
        public Signal<ulong> ClientDisconnected = new();
    }
}

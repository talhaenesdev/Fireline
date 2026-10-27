using FlowIoC.BaseModule.Signals;
using Modules.LobbyModule.Shared.Data.ValueObjects;

namespace Modules.LobbyModule.LobbyScreenModule.Signals
{
    public class LobbyScreenSignals : ISignalHolder
    {
        public LobbyScreenSignalsIncoming Incoming = new();
        public LobbyScreenSignalsOutgoing Outgoing = new();
    }

    public class LobbyScreenSignalsIncoming
    {
        /// <summary>Opens the panel when it is closed and closes it when it is open.</summary>
        public Signal Toggle = new();

        public Signal<LobbyStateVO> ApplyState = new();
    }

    public class LobbyScreenSignalsOutgoing
    {
        /// <summary>The player name typed in the panel.</summary>
        public Signal<string> CreateClicked = new();

        /// <summary>The player name, then the join code.</summary>
        public Signal<string, string> JoinClicked = new();

        public Signal LeaveClicked = new();
        public Signal ReadyClicked = new();
        public Signal StartClicked = new();
        public Signal ModeClicked = new();
    }
}

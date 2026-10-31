using FlowIoC.BaseModule.Connectors;
using FlowIoC.BaseModule.Contexts;
using Modules.LobbyModule.LobbyScreenModule.Signals;
using Modules.LobbyModule.Signals;
using Modules.MainModule.MainScreenModule.Signals;
using Modules.NetworkSessionModule.Signals;

namespace Modules.ConnectorModule.RootsContexts
{
    public class LobbyConnectorSubContext : Context
    {
        private const string GROUP = nameof(LobbyConnectorSubContext);

        private LobbySignals _lobbySignals;
        private LobbyScreenSignals _lobbyScreenSignals;
        private MainScreenSignals _mainScreenSignals;
        private NetworkSessionSignals _networkSessionSignals;

        public override void Setup()
        {
            base.Setup();

            _lobbySignals = InjectionBinderCrossContext.GetInstance<LobbySignals>();
            _lobbyScreenSignals = InjectionBinderCrossContext.GetInstance<LobbyScreenSignals>();
            _mainScreenSignals = InjectionBinderCrossContext.GetInstance<MainScreenSignals>();
            _networkSessionSignals = InjectionBinderCrossContext.GetInstance<NetworkSessionSignals>();

            IncomingSignals();
            OutgoingSignals();
        }

        private void IncomingSignals()
        {
            _mainScreenSignals.Outgoing.LobbyClicked.Connect(_lobbyScreenSignals.Incoming.Toggle, GROUP);

            _lobbyScreenSignals.Outgoing.CreateClicked.Connect(_lobbySignals.Incoming.CreateLobby, GROUP);
            _lobbyScreenSignals.Outgoing.JoinClicked.Connect(_lobbySignals.Incoming.JoinLobby, GROUP);
            _lobbyScreenSignals.Outgoing.LeaveClicked.Connect(_lobbySignals.Incoming.LeaveLobby, GROUP);
            _lobbyScreenSignals.Outgoing.ReadyClicked.Connect(_lobbySignals.Incoming.ToggleReady, GROUP);
            _lobbyScreenSignals.Outgoing.StartClicked.Connect(_lobbySignals.Incoming.StartGame, GROUP);
            _lobbyScreenSignals.Outgoing.ModeClicked.Connect(_lobbySignals.Incoming.CycleGameMode, GROUP);
            _lobbyScreenSignals.Outgoing.DifficultyClicked.Connect(_lobbySignals.Incoming.CycleDifficulty, GROUP);
            _lobbyScreenSignals.Outgoing.MapClicked.Connect(_lobbySignals.Incoming.CycleMap, GROUP);

            _networkSessionSignals.Outgoing.SessionChanged.Connect(_lobbySignals.Incoming.SessionChanged, GROUP);
        }

        private void OutgoingSignals()
        {
            _lobbySignals.Outgoing.StateChanged.Connect(_lobbyScreenSignals.Incoming.ApplyState, GROUP);
        }

        public override void DestroyContext()
        {
            SignalConnector.DisconnectGroup(GROUP);
            base.DestroyContext();
        }
    }
}

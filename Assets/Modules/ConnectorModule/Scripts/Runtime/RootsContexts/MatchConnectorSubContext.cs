using FlowIoC.BaseModule.Connectors;
using FlowIoC.BaseModule.Contexts;
using Modules.MatchModule.MatchScreenModule.Signals;
using Modules.MatchModule.Signals;
using Modules.NetworkSessionModule.Signals;
using Modules.PlayerModule.HudScreenModule.Signals;
using Modules.PlayerModule.Signals;

namespace Modules.ConnectorModule.RootsContexts
{
    /// <summary>The match scene's rules: who joins, who dies, the screens that open with it, and leaving.</summary>
    public class MatchConnectorSubContext : Context
    {
        private const string GROUP = nameof(MatchConnectorSubContext);

        private MatchSignals _matchSignals;
        private MatchScreenSignals _matchScreenSignals;
        private HudScreenSignals _hudScreenSignals;
        private PlayerSignals _playerSignals;
        private NetworkSessionSignals _networkSessionSignals;

        public override void Setup()
        {
            base.Setup();

            _matchSignals = InjectionBinderCrossContext.GetInstance<MatchSignals>();
            _matchScreenSignals = InjectionBinderCrossContext.GetInstance<MatchScreenSignals>();
            _hudScreenSignals = InjectionBinderCrossContext.GetInstance<HudScreenSignals>();
            _playerSignals = InjectionBinderCrossContext.GetInstance<PlayerSignals>();
            _networkSessionSignals = InjectionBinderCrossContext.GetInstance<NetworkSessionSignals>();

            IncomingSignals();
            OutgoingSignals();
        }

        private void IncomingSignals()
        {
            _networkSessionSignals.Outgoing.ClientConnected.Connect(_matchSignals.Incoming.ClientConnected, GROUP);
            _networkSessionSignals.Outgoing.ClientDisconnected.Connect(_matchSignals.Incoming.ClientDisconnected, GROUP);

            _playerSignals.Outgoing.PlayerDied.Connect(_matchSignals.Incoming.PlayerDied, GROUP);
            _playerSignals.Outgoing.PlayerRevived.Connect(_matchSignals.Incoming.PlayerRevived, GROUP);
            _playerSignals.Outgoing.PlayerNameChanged.Connect(_matchSignals.Incoming.PlayerNameChanged, GROUP);

            _matchScreenSignals.Outgoing.ExitClicked.Connect(_matchSignals.Incoming.ExitMatch, GROUP);
            _matchScreenSignals.Outgoing.CameraSettingsEdited.Connect(_playerSignals.Incoming.SetCameraSettings, GROUP);
        }

        private void OutgoingSignals()
        {
            _matchSignals.Outgoing.Started.Connect(_hudScreenSignals.Incoming.Open, GROUP);
            _matchSignals.Outgoing.Started.Connect(_matchScreenSignals.Incoming.Open, GROUP);
            _matchSignals.Outgoing.ScoresChanged.Connect(_matchScreenSignals.Incoming.ApplyScores, GROUP);
            _matchSignals.Outgoing.GameOver.Connect(_matchScreenSignals.Incoming.ShowGameOver, GROUP);
            _matchSignals.Outgoing.FriendlyFireChanged.Connect(_playerSignals.Incoming.SetFriendlyFire, GROUP);
            _playerSignals.Outgoing.CameraSettingsChanged.Connect(_matchScreenSignals.Incoming.ApplyCameraSettings, GROUP);
        }

        public override void DestroyContext()
        {
            SignalConnector.DisconnectGroup(GROUP);
            base.DestroyContext();
        }
    }
}

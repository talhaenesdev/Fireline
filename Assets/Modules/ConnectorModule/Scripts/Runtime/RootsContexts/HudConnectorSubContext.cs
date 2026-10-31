using FlowIoC.BaseModule.Connectors;
using FlowIoC.BaseModule.Contexts;
using Modules.PlayerModule.HudScreenModule.Signals;
using Modules.PlayerModule.Signals;
using Modules.WeaponModule.Signals;

namespace Modules.ConnectorModule.RootsContexts
{
    public class HudConnectorSubContext : Context
    {
        private const string GROUP = nameof(HudConnectorSubContext);

        private PlayerSignals _playerSignals;
        private HudScreenSignals _hudScreenSignals;
        private WeaponSignals _weaponSignals;

        public override void Setup()
        {
            base.Setup();

            _playerSignals = InjectionBinderCrossContext.GetInstance<PlayerSignals>();
            _hudScreenSignals = InjectionBinderCrossContext.GetInstance<HudScreenSignals>();
            _weaponSignals = InjectionBinderCrossContext.GetInstance<WeaponSignals>();

            IncomingSignals();
            OutgoingSignals();
        }

        private void IncomingSignals()
        {
            _playerSignals.Outgoing.LocalHealthChanged.Connect(_hudScreenSignals.Incoming.ApplyHealth, GROUP);
            _playerSignals.Outgoing.LocalHurt.Connect(_hudScreenSignals.Incoming.ApplyHurt, GROUP);
            _playerSignals.Outgoing.MarkersChanged.Connect(_hudScreenSignals.Incoming.ApplyMarkers, GROUP);
            _weaponSignals.Outgoing.LocalWeaponChanged.Connect(_hudScreenSignals.Incoming.ApplyWeaponName, GROUP);
            _playerSignals.Outgoing.LocalShieldChanged.Connect(_hudScreenSignals.Incoming.ApplyShield, GROUP);
            _playerSignals.Outgoing.ReviveStateChanged.Connect(_hudScreenSignals.Incoming.ApplyRevive, GROUP);
        }

        // A phone's on-screen controls drive the same player and weapon the keyboard and mouse do.
        private void OutgoingSignals()
        {
            _hudScreenSignals.Outgoing.TouchMove.Connect(_playerSignals.Incoming.TouchMove, GROUP);
            _hudScreenSignals.Outgoing.TouchFire.Connect(_weaponSignals.Incoming.FireInput, GROUP);
            _hudScreenSignals.Outgoing.TouchReload.Connect(_weaponSignals.Incoming.ReloadInput, GROUP);
            _hudScreenSignals.Outgoing.ReviveClicked.Connect(_playerSignals.Incoming.StartRevive, GROUP);
            _hudScreenSignals.Outgoing.PingClicked.Connect(_playerSignals.Incoming.Ping, GROUP);
        }

        public override void DestroyContext()
        {
            SignalConnector.DisconnectGroup(GROUP);
            base.DestroyContext();
        }
    }
}

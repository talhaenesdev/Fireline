using FlowIoC.BaseModule.Connectors;
using FlowIoC.BaseModule.Contexts;
using Modules.PlayerModule.HudScreenModule.Signals;
using Modules.PlayerModule.Signals;
using Modules.WeaponModule.Signals;

namespace Modules.ConnectorModule.RootsContexts
{
    /// <summary>The player's hands on the weapon, and what a shot does to the player hit and the HUD.</summary>
    public class WeaponConnectorSubContext : Context
    {
        private const string GROUP = nameof(WeaponConnectorSubContext);

        private WeaponSignals _weaponSignals;
        private PlayerSignals _playerSignals;
        private HudScreenSignals _hudScreenSignals;

        public override void Setup()
        {
            base.Setup();

            _weaponSignals = InjectionBinderCrossContext.GetInstance<WeaponSignals>();
            _playerSignals = InjectionBinderCrossContext.GetInstance<PlayerSignals>();
            _hudScreenSignals = InjectionBinderCrossContext.GetInstance<HudScreenSignals>();

            IncomingSignals();
            OutgoingSignals();
        }

        private void IncomingSignals()
        {
            _playerSignals.Outgoing.FireInput.Connect(_weaponSignals.Incoming.FireInput, GROUP);
            _playerSignals.Outgoing.ReloadInput.Connect(_weaponSignals.Incoming.ReloadInput, GROUP);
            _playerSignals.Outgoing.AimChanged.Connect(_weaponSignals.Incoming.AimChanged, GROUP);
            _playerSignals.Outgoing.LocalAliveChanged.Connect(_weaponSignals.Incoming.SetArmed, GROUP);
        }

        private void OutgoingSignals()
        {
            _weaponSignals.Outgoing.LocalFired.Connect(_playerSignals.Incoming.ShakeCamera, GROUP);
            _weaponSignals.Outgoing.PlayerHit.Connect(_playerSignals.Incoming.ApplyDamage, GROUP);
            _weaponSignals.Outgoing.AmmoChanged.Connect(_hudScreenSignals.Incoming.ApplyAmmo, GROUP);
        }

        public override void DestroyContext()
        {
            SignalConnector.DisconnectGroup(GROUP);
            base.DestroyContext();
        }
    }
}

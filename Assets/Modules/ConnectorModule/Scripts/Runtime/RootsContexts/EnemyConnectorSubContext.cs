using FlowIoC.BaseModule.Connectors;
using FlowIoC.BaseModule.Contexts;
using Modules.EnemyModule.Signals;
using Modules.MatchModule.Signals;
using Modules.PlayerModule.Signals;
using Modules.WeaponModule.Signals;

namespace Modules.ConnectorModule.RootsContexts
{
    /// <summary>The zombie match: Match starts the waves, rounds hit zombies, zombies bite players and score kills.</summary>
    public class EnemyConnectorSubContext : Context
    {
        private const string GROUP = nameof(EnemyConnectorSubContext);

        private EnemySignals _enemySignals;
        private MatchSignals _matchSignals;
        private PlayerSignals _playerSignals;
        private WeaponSignals _weaponSignals;

        public override void Setup()
        {
            base.Setup();

            _enemySignals = InjectionBinderCrossContext.GetInstance<EnemySignals>();
            _matchSignals = InjectionBinderCrossContext.GetInstance<MatchSignals>();
            _playerSignals = InjectionBinderCrossContext.GetInstance<PlayerSignals>();
            _weaponSignals = InjectionBinderCrossContext.GetInstance<WeaponSignals>();

            IncomingSignals();
            OutgoingSignals();
        }

        private void IncomingSignals()
        {
            _matchSignals.Outgoing.ZombieWavesStarted.Connect(_enemySignals.Incoming.StartWaves, GROUP);
            _weaponSignals.Outgoing.ObjectHit.Connect(_enemySignals.Incoming.DamageZombie, GROUP);
        }

        private void OutgoingSignals()
        {
            _enemySignals.Outgoing.PlayerAttacked.Connect(_playerSignals.Incoming.ApplyDamage, GROUP);
            _enemySignals.Outgoing.ZombieKilled.Connect(_matchSignals.Incoming.ZombieKilled, GROUP);
        }

        public override void DestroyContext()
        {
            SignalConnector.DisconnectGroup(GROUP);
            base.DestroyContext();
        }
    }
}

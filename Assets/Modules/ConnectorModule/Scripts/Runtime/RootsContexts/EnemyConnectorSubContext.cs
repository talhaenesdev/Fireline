using FlowIoC.BaseModule.Connectors;
using FlowIoC.BaseModule.Contexts;
using Modules.EnemyModule.Signals;
using Modules.MatchModule.Signals;
using Modules.PlayerModule.HudScreenModule.Signals;
using Modules.PlayerModule.Signals;
using Modules.WeaponModule.Signals;

namespace Modules.ConnectorModule.RootsContexts
{
    /// <summary>
    /// The zombie match: Match starts and ends the waves, rounds hit zombies, zombies bite players and
    /// score kills, and the HUD shows the wave.
    /// </summary>
    public class EnemyConnectorSubContext : Context
    {
        private const string GROUP = nameof(EnemyConnectorSubContext);

        private EnemySignals _enemySignals;
        private MatchSignals _matchSignals;
        private PlayerSignals _playerSignals;
        private WeaponSignals _weaponSignals;
        private HudScreenSignals _hudScreenSignals;

        public override void Setup()
        {
            base.Setup();

            _enemySignals = InjectionBinderCrossContext.GetInstance<EnemySignals>();
            _matchSignals = InjectionBinderCrossContext.GetInstance<MatchSignals>();
            _playerSignals = InjectionBinderCrossContext.GetInstance<PlayerSignals>();
            _weaponSignals = InjectionBinderCrossContext.GetInstance<WeaponSignals>();
            _hudScreenSignals = InjectionBinderCrossContext.GetInstance<HudScreenSignals>();

            IncomingSignals();
            OutgoingSignals();
        }

        private void IncomingSignals()
        {
            _matchSignals.Outgoing.ZombieDifficultyChosen.Connect(_enemySignals.Incoming.SetDifficulty, GROUP);
            _matchSignals.Outgoing.ZombieWavesStarted.Connect(_enemySignals.Incoming.StartWaves, GROUP);
            _matchSignals.Outgoing.GameOver.Connect(_enemySignals.Incoming.StopWaves, GROUP);
            _weaponSignals.Outgoing.ObjectHit.Connect(_enemySignals.Incoming.DamageZombie, GROUP);
            _playerSignals.Outgoing.PlayerDied.Connect(_enemySignals.Incoming.PlayerDied, GROUP);
            _playerSignals.Outgoing.PlayerRevived.Connect(_enemySignals.Incoming.PlayerRevived, GROUP);
            _hudScreenSignals.Outgoing.PlaceTurretClicked.Connect(_enemySignals.Incoming.ConfirmTurretPlacement, GROUP);
        }

        private void OutgoingSignals()
        {
            _enemySignals.Outgoing.PlayerAttacked.Connect(_playerSignals.Incoming.ApplyDamage, GROUP);
            _enemySignals.Outgoing.ZombieKilled.Connect(_matchSignals.Incoming.ZombieKilled, GROUP);
            _enemySignals.Outgoing.WaveStateChanged.Connect(_hudScreenSignals.Incoming.ApplyWave, GROUP);
            _enemySignals.Outgoing.BossStateChanged.Connect(_hudScreenSignals.Incoming.ApplyBoss, GROUP);
            _enemySignals.Outgoing.ObjectiveChanged.Connect(_hudScreenSignals.Incoming.ApplyObjective, GROUP);
            _enemySignals.Outgoing.ObjectiveEnded.Connect(_hudScreenSignals.Incoming.ApplyObjectiveEnded, GROUP);
            _enemySignals.Outgoing.TurretPlacementChanged.Connect(_hudScreenSignals.Incoming.ApplyTurretPlacement, GROUP);
        }

        public override void DestroyContext()
        {
            SignalConnector.DisconnectGroup(GROUP);
            base.DestroyContext();
        }
    }
}

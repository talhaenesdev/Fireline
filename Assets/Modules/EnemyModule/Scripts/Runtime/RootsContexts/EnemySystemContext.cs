using FlowIoC.BaseModule.Contexts;
using FlowIoC.BaseModule.Controller.Commands;
using Modules.EnemyModule.Controllers;
using Modules.EnemyModule.Models;
using Modules.EnemyModule.Signals;
using Modules.EnemyModule.ViewsMediators;

namespace Modules.EnemyModule.RootsContexts
{
    public class EnemySystemContext : Context
    {
        private EnemySignals _signals;
        private EnemyInternalSignals _internalSignals;
        private IEnemyModel _model;

        public override void SignalBindings()
        {
            base.SignalBindings();
            _signals = InjectionBinderCrossContext.Bind<EnemySignals>();
            _internalSignals = InjectionBinder.Bind<EnemyInternalSignals>();
        }

        public override void InjectionBindings()
        {
            base.InjectionBindings();
            _model = InjectionBinder.Bind<IEnemyModel, EnemyModel>();
        }

        public override void MediationBindings()
        {
            base.MediationBindings();
            MediationBinder.Bind<ZombieWavesView>().To<ZombieWavesMediator>();
        }

        public override void CommandBindings()
        {
            base.CommandBindings();

            CommandBinder.Bind(_internalSignals.BoardRegistered)
                .ToSequence<StoreBoardCommand>()
                .ToSequence<PoolZombiesCommand>()
                .ToSequence<PoolAlliesCommand>();

            CommandBinder.Bind(_signals.Incoming.StartWaves)
                .ToSequence<RequireServerCommand>()
                .ToSequence<BeginWavesCommand>()
                .ToSequence<RaiseBaseWallsCommand>()
                .ToSequence<StartFrameTickCommand>()
                .ToSequence<SignalDispatchCommand>(_internalSignals.NextWave);

            // One wave a turn, each turn waiting for the last to be cleared.
            CommandBinder.Bind(_internalSignals.NextWave)
                .ToSequence<WaitWaveDelayCommand>()
                .ToSequence<SpawnWaveCommand>()
                .ToSequence<StartObjectiveCommand>()
                .ToSequence<WaitWaveClearedCommand>()
                .ToSequence<EndObjectiveCommand>()
                .ToSequence<AnnounceWaveClearedCommand>()
                .ToSequence<SignalDispatchCommand>(_internalSignals.NextWave);

            CommandBinder.Bind(_internalSignals.Tick)
                .ToSequence<ChaseAndAttackCommand>()
                .ToSequence<MoveSpitCommand>()
                .ToSequence<HoldObjectiveCommand>()
                .ToSequence<TurretsFireCommand>()
                .ToSequence<TrainAlliesCommand>()
                .ToSequence<AlliesFightCommand>();

            CommandBinder.Bind(_signals.Incoming.GrantTurret)
                .ToSequence<RequireServerCommand>()
                .ToSequence<GrantTurretCommand>();

            CommandBinder.Bind(_signals.Incoming.GrantBarricade)
                .ToSequence<RequireServerCommand>()
                .ToSequence<GrantBarricadeCommand>();

            CommandBinder.Bind(_signals.Incoming.GrantWall)
                .ToSequence<RequireServerCommand>()
                .ToSequence<GrantWallCommand>();

            CommandBinder.Bind(_signals.Incoming.GrantBarracks)
                .ToSequence<RequireServerCommand>()
                .ToSequence<GrantBarracksCommand>();

            CommandBinder.Bind(_signals.Incoming.UpgradeTurrets)
                .ToSequence<RequireServerCommand>()
                .ToSequence<UpgradeTurretsCommand>();

            CommandBinder.Bind(_internalSignals.StructurePlaceRequested)
                .ToSequence<RequireServerCommand>()
                .ToSequence<PlaceStructureCommand>();

            CommandBinder.Bind(_internalSignals.StructureGranted).ToSequence<BeginStructurePlacementCommand>();
            CommandBinder.Bind(_signals.Incoming.ConfirmTurretPlacement).ToSequence<ConfirmStructurePlacementCommand>();

            CommandBinder.Bind(_signals.Incoming.SetDifficulty).ToSequence<StoreDifficultyCommand>();

            CommandBinder.Bind(_signals.Incoming.StopWaves)
                .ToSequence<RequireServerCommand>()
                .ToSequence<StopWavesCommand>();

            CommandBinder.Bind(_signals.Incoming.PlayerDied)
                .ToSequence<RequireServerCommand>()
                .ToSequence<MarkPlayerDownCommand>();

            CommandBinder.Bind(_signals.Incoming.PlayerRevived)
                .ToSequence<RequireServerCommand>()
                .ToSequence<MarkPlayerUpCommand>();

            CommandBinder.Bind(_signals.Incoming.DamageZombie)
                .ToSequence<RequireServerCommand>()
                .ToSequence<DamageZombieCommand>();
        }

        public override void DestroyContext()
        {
            // The update loop and the coroutine provider outlive the scene; the flags let both let go.
            if (_model != null)
            {
                _model.IsRunning = false;
                _model.IsTicking = false;

                // The handler hands out this scene's pool; the next scene registers its own.
                if (_model.PooledZombiePrefab != null && Unity.Netcode.NetworkManager.Singleton != null)
                    Unity.Netcode.NetworkManager.Singleton.PrefabHandler.RemoveHandler(_model.PooledZombiePrefab);

                if (_model.PooledAllyPrefab != null && Unity.Netcode.NetworkManager.Singleton != null)
                    Unity.Netcode.NetworkManager.Singleton.PrefabHandler.RemoveHandler(_model.PooledAllyPrefab);
            }

            base.DestroyContext();
        }
    }
}

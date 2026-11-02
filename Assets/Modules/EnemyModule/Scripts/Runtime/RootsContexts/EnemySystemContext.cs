using FlowIoC.BaseModule.Contexts;
using FlowIoC.BaseModule.Controller.Commands;
using Modules.AudioModule.Services;
using Modules.AudioModule.Shared;
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

            CommandBinder.Bind(_internalSignals.ZombieHitShown).ToSequence<IAudioService.Commands.PlayAt>(AudioKey.Enemy.ZombieHit);
            CommandBinder.Bind(_internalSignals.ZombieDeathShown).ToSequence<IAudioService.Commands.PlayAt>(AudioKey.Enemy.ZombieDeath);
            CommandBinder.Bind(_internalSignals.BlastShown)
                .ToSequence<IAudioService.Commands.PlayAt>(AudioKey.Enemy.Explosion)
                .ToSequence<AnnounceRumbleCommand>();
            CommandBinder.Bind(_internalSignals.StructureFellShown)
                .ToSequence<IAudioService.Commands.PlayAt>(AudioKey.Enemy.StructureBreak)
                .ToSequence<AnnounceRumbleCommand>();
            CommandBinder.Bind(_internalSignals.WaveClearedShown).ToSequence<IAudioService.Commands.Play>(AudioKey.Enemy.WaveCleared);

            CommandBinder.Bind(_internalSignals.PoolRequested).ToSequence<ConfigureBoardPoolCommand>();
            CommandBinder.Bind(_internalSignals.BoardRegistered)
                .ToSequence<StoreBoardCommand>()
                .ToSequence<PoolZombiesCommand>()
                .ToSequence<PoolAlliesCommand>();

            CommandBinder.Bind(_signals.Incoming.StartWaves)
                .ToSequence<RequireServerCommand>()
                .ToSequence<BeginWavesCommand>()
                .ToSequence<RaiseBaseWallsCommand>()
                .ToSequence<RaiseHeadquartersCommand>()
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
                .ToSequence<RepairStationsMendCommand>()
                .ToSequence<TrapsCommand>()
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

            CommandBinder.Bind(_signals.Incoming.GrantGate)
                .ToSequence<RequireServerCommand>()
                .ToSequence<GrantGateCommand>();

            CommandBinder.Bind(_signals.Incoming.GrantRepairStation)
                .ToSequence<RequireServerCommand>()
                .ToSequence<GrantRepairStationCommand>();

            CommandBinder.Bind(_signals.Incoming.GrantSpikeTrap)
                .ToSequence<RequireServerCommand>()
                .ToSequence<GrantSpikeTrapCommand>();

            CommandBinder.Bind(_signals.Incoming.GrantMine)
                .ToSequence<RequireServerCommand>()
                .ToSequence<GrantMineCommand>();

            CommandBinder.Bind(_signals.Incoming.ReinforceWalls)
                .ToSequence<RequireServerCommand>()
                .ToSequence<ReinforceWallsCommand>();

            CommandBinder.Bind(_signals.Incoming.RequestRepair).ToSequence<RequestRepairCommand>();
            CommandBinder.Bind(_signals.Incoming.ToggleAllyOrder).ToSequence<RequestAllyOrderCommand>();

            CommandBinder.Bind(_internalSignals.AllyOrderRequested)
                .ToSequence<RequireServerCommand>()
                .ToSequence<ToggleAllyOrderCommand>();

            CommandBinder.Bind(_signals.Incoming.UpgradeBarracks)
                .ToSequence<RequireServerCommand>()
                .ToSequence<UpgradeBarracksCommand>();

            CommandBinder.Bind(_internalSignals.RepairRequested)
                .ToSequence<RequireServerCommand>()
                .ToSequence<QuoteRepairCommand>();

            CommandBinder.Bind(_signals.Incoming.RepairStructure)
                .ToSequence<RequireServerCommand>()
                .ToSequence<RepairStructureCommand>();

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

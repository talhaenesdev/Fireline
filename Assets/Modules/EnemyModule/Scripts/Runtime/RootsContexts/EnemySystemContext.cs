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

            CommandBinder.Bind(_internalSignals.BoardRegistered).ToSequence<StoreBoardCommand>();

            CommandBinder.Bind(_signals.Incoming.StartWaves)
                .ToSequence<RequireServerCommand>()
                .ToSequence<BeginWavesCommand>()
                .ToSequence<StartFrameTickCommand>()
                .ToSequence<SignalDispatchCommand>(_internalSignals.NextWave);

            // One wave a turn, each turn waiting for the last to be cleared.
            CommandBinder.Bind(_internalSignals.NextWave)
                .ToSequence<WaitWaveDelayCommand>()
                .ToSequence<SpawnWaveCommand>()
                .ToSequence<WaitWaveClearedCommand>()
                .ToSequence<SignalDispatchCommand>(_internalSignals.NextWave);

            CommandBinder.Bind(_internalSignals.Tick)
                .ToSequence<ChaseAndAttackCommand>()
                .ToSequence<TurretsFireCommand>();

            CommandBinder.Bind(_signals.Incoming.PlaceTurret)
                .ToSequence<RequireServerCommand>()
                .ToSequence<PlaceTurretCommand>();

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
            }

            base.DestroyContext();
        }
    }
}

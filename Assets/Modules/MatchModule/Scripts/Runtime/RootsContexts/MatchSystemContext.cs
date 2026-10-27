using FlowIoC.BaseModule.Contexts;
using Modules.MatchModule.Controllers;
using Modules.MatchModule.Models;
using Modules.MatchModule.Signals;
using Modules.MatchModule.ViewsMediators;

namespace Modules.MatchModule.RootsContexts
{
    public class MatchSystemContext : Context
    {
        private MatchSignals _signals;
        private MatchInternalSignals _internalSignals;

        public override void SignalBindings()
        {
            base.SignalBindings();
            _internalSignals = InjectionBinder.Bind<MatchInternalSignals>();
            _signals = InjectionBinderCrossContext.Bind<MatchSignals>();
        }

        public override void InjectionBindings()
        {
            base.InjectionBindings();
            InjectionBinder.Bind<IMatchModel, MatchModel>();
        }

        public override void MediationBindings()
        {
            base.MediationBindings();
            MediationBinder.Bind<MatchArenaView>().To<MatchArenaMediator>();
            MediationBinder.Bind<ScoreboardNetworkView>().To<ScoreboardNetworkMediator>();
        }

        public override void CommandBindings()
        {
            base.CommandBindings();

            // A player spawned before every Setup has run would announce itself to unwired connectors,
            // so spawning waits for Launch as well as for the arena.
            CommandBinder.Bind(_internalSignals.Launched)
                .ToSequence<MarkLaunchedCommand>()
                .ToSequence<RequireServerCommand>()
                .ToSequence<ReadGameModeCommand>()
                .ToSequence<StartGameModeCommand>()
                .ToSequence<SpawnConnectedPlayersCommand>();

            CommandBinder.Bind(_internalSignals.ArenaRegistered)
                .ToSequence<StoreArenaCommand>()
                .ToSequence<RequireServerCommand>()
                .ToSequence<SpawnConnectedPlayersCommand>();

            CommandBinder.Bind(_internalSignals.ScoreboardRegistered)
                .ToSequence<StoreScoreboardCommand>()
                .ToSequence<RequireServerCommand>()
                .ToSequence<AddConnectedScoresCommand>();

            CommandBinder.Bind(_signals.Incoming.ClientConnected)
                .ToSequence<RequireServerCommand>()
                .ToSequence<AddScoreCommand>()
                .ToSequence<SpawnPlayerCommand>();

            CommandBinder.Bind(_signals.Incoming.ClientDisconnected)
                .ToSequence<RequireServerCommand>()
                .ToSequence<RemoveScoreCommand>();

            CommandBinder.Bind(_signals.Incoming.PlayerNameChanged)
                .ToSequence<RequireServerCommand>()
                .ToSequence<UpdateScoreNameCommand>();

            CommandBinder.Bind(_signals.Incoming.PlayerDied)
                .ToSequence<RequireServerCommand>()
                .ToSequence<RecordKillCommand>()
                .ToSequence<WaitRespawnDelayCommand>()
                .ToSequence<RespawnPlayerCommand>();

            CommandBinder.Bind(_signals.Incoming.ZombieKilled)
                .ToSequence<RequireServerCommand>()
                .ToSequence<AddZombieKillCommand>();

            CommandBinder.Bind(_signals.Incoming.ExitMatch)
                .ToSequence<LeaveNetworkCommand>()
                .ToSequence<LoadMainSceneCommand>();
        }

        public override void Launch()
        {
            base.Launch();
            _signals.Outgoing.Started.Dispatch();
            _internalSignals.Launched.Dispatch();
        }
    }
}

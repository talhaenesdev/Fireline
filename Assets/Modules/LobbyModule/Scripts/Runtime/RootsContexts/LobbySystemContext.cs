using FlowIoC.BaseModule.Contexts;
using Modules.LobbyModule.Controllers;
using Modules.LobbyModule.Models;
using Modules.LobbyModule.Shared.Constants;
using Modules.LobbyModule.Signals;

namespace Modules.LobbyModule.RootsContexts
{
    public class LobbySystemContext : Context
    {
        private LobbySignals _signals;
        private LobbyInternalSignals _internalSignals;

        public override void SignalBindings()
        {
            base.SignalBindings();
            _signals = InjectionBinderCrossContext.Bind<LobbySignals>();
            _internalSignals = InjectionBinder.Bind<LobbyInternalSignals>();
        }

        public override void InjectionBindings()
        {
            base.InjectionBindings();
            InjectionBinder.Bind<ILobbyModel, LobbyModel>();
        }

        public override void CommandBindings()
        {
            base.CommandBindings();

            CommandBinder.Bind(_internalSignals.Launch).ToSequence<InitializeServicesCommand>();

            CommandBinder.Bind(_signals.Incoming.CreateLobby)
                .ToSequence<ClaimLobbyCommand>(LobbyConstants.STATUS_CREATING)
                .ToSequence<SavePlayerNameCommand>()
                .ToSequence<CreateSessionCommand>()
                .ToSequence<WriteGameModeCommand>()
                .ToSequence<FinishLobbyCommand>(LobbyConstants.STATUS_CREATED);

            CommandBinder.Bind(_signals.Incoming.JoinLobby)
                .ToSequence<ClaimLobbyCommand>(LobbyConstants.STATUS_JOINING)
                .ToSequence<SavePlayerNameCommand>()
                .ToSequence<JoinSessionCommand>()
                .ToSequence<ReadGameModeCommand>()
                .ToSequence<FinishLobbyCommand>(LobbyConstants.STATUS_JOINED);

            CommandBinder.Bind(_signals.Incoming.LeaveLobby)
                .ToSequence<ClaimLobbyCommand>(LobbyConstants.STATUS_LEAVING)
                .ToSequence<LeaveSessionCommand>()
                .ToSequence<FinishLobbyCommand>(LobbyConstants.STATUS_LEFT);

            CommandBinder.Bind(_signals.Incoming.ToggleReady)
                .ToSequence<ClaimLobbyCommand>((string) null)
                .ToSequence<ToggleReadyCommand>()
                .ToSequence<FinishLobbyCommand>((string) null);

            // The busy flag stays set on success: the scene is about to change under the lobby.
            CommandBinder.Bind(_signals.Incoming.StartGame)
                .ToSequence<ClaimLobbyCommand>(LobbyConstants.STATUS_STARTING)
                .ToSequence<CheckCanStartCommand>()
                .ToSequence<StartHostNetworkCommand>()
                .ToSequence<LoadGameSceneCommand>();

            CommandBinder.Bind(_signals.Incoming.CycleGameMode)
                .ToSequence<ClaimLobbyCommand>((string) null)
                .ToSequence<CycleGameModeCommand>()
                .ToSequence<FinishLobbyCommand>((string) null);

            CommandBinder.Bind(_signals.Incoming.SessionChanged)
                .ToSequence<ReadGameModeCommand>()
                .ToSequence<StoreSessionCommand>();
        }

        public override void Launch()
        {
            base.Launch();
            _internalSignals.Launch.Dispatch();
        }
    }
}

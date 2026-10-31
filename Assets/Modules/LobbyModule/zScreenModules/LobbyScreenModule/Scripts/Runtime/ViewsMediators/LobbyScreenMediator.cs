using FlowIoC.BaseModule.Injectable.Attributes;
using FlowIoC.BaseModule.ViewsMediators.Mediator;
using FlowIoC.ScreenModule.Enums;
using FlowIoC.ScreenModule.Extensions;
using FlowIoC.ScreenModule.ViewsMediators.Screen;
using Modules.LobbyModule.LobbyScreenModule.Signals;
using Modules.LobbyModule.Shared.Data.ValueObjects;

namespace Modules.LobbyModule.LobbyScreenModule.ViewsMediators
{
    public class LobbyScreenMediator : IMediator
    {
        [Inject] private LobbyScreenView _view { get; set; }
        [InjectSignal] private LobbyScreenSignals _signals { get; set; }

        public void OnRegister()
        {
            _view.ShowCompleted += OnScreenShown;
            _view.HideCompleted += OnScreenHidden;
        }

        public void OnRemove()
        {
            _view.ShowCompleted -= OnScreenShown;
            _view.HideCompleted -= OnScreenHidden;
        }

        private void OnScreenShown(IScreenBody screen)
        {
            _view.CreateClicked += OnCreateClicked;
            _view.JoinClicked += OnJoinClicked;
            _view.LeaveClicked += OnLeaveClicked;
            _view.ReadyClicked += OnReadyClicked;
            _view.StartClicked += OnStartClicked;
            _view.ModeClicked += OnModeClicked;
            _view.DifficultyClicked += OnDifficultyClicked;
            _view.MapClicked += OnMapClicked;
            _signals.Incoming.ApplyState.AddListener(OnStateChanged);
        }

        private void OnScreenHidden(IScreenBody screen)
        {
            _view.CreateClicked -= OnCreateClicked;
            _view.JoinClicked -= OnJoinClicked;
            _view.LeaveClicked -= OnLeaveClicked;
            _view.ReadyClicked -= OnReadyClicked;
            _view.StartClicked -= OnStartClicked;
            _view.ModeClicked -= OnModeClicked;
            _view.DifficultyClicked -= OnDifficultyClicked;
            _view.MapClicked -= OnMapClicked;
            _signals.Incoming.ApplyState.RemoveListener(OnStateChanged);
        }

        private void OnStateChanged(LobbyStateVO state) => _view.ShowState(state);

        private void OnCreateClicked(string playerName)
        {
            if (CanSend())
                _signals.Outgoing.CreateClicked.Dispatch(playerName);
        }

        private void OnJoinClicked(string playerName, string joinCode)
        {
            if (CanSend())
                _signals.Outgoing.JoinClicked.Dispatch(playerName, joinCode);
        }

        private void OnLeaveClicked()
        {
            if (CanSend())
                _signals.Outgoing.LeaveClicked.Dispatch();
        }

        private void OnReadyClicked()
        {
            if (CanSend())
                _signals.Outgoing.ReadyClicked.Dispatch();
        }

        private void OnStartClicked()
        {
            if (CanSend())
                _signals.Outgoing.StartClicked.Dispatch();
        }

        private void OnModeClicked()
        {
            if (CanSend())
                _signals.Outgoing.ModeClicked.Dispatch();
        }

        private void OnDifficultyClicked()
        {
            if (CanSend())
                _signals.Outgoing.DifficultyClicked.Dispatch();
        }

        private void OnMapClicked()
        {
            if (CanSend())
                _signals.Outgoing.MapClicked.Dispatch();
        }

        private bool CanSend() => _view.Data.HasState(ScreenState.AvailableToSendSignal);
    }
}

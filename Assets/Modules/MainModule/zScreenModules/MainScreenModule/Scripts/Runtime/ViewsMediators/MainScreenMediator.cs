using FlowIoC.BaseModule.Injectable.Attributes;
using FlowIoC.BaseModule.ViewsMediators.Mediator;
using FlowIoC.ScreenModule.Enums;
using FlowIoC.ScreenModule.Extensions;
using FlowIoC.ScreenModule.ViewsMediators.Screen;
using Modules.MainModule.MainScreenModule.Signals;

namespace Modules.MainModule.MainScreenModule.ViewsMediators
{
    public class MainScreenMediator : IMediator
    {
        [Inject] private MainScreenView _view { get; set; }
        [InjectSignal] private MainScreenSignals _signals { get; set; }

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
            _view.LobbyClicked += OnLobbyClicked;
            _view.QuitClicked += OnQuitClicked;
        }

        private void OnScreenHidden(IScreenBody screen)
        {
            _view.LobbyClicked -= OnLobbyClicked;
            _view.QuitClicked -= OnQuitClicked;
        }

        // The menu stays up behind the lobby panel, which opens on a layer above it.
        private void OnLobbyClicked()
        {
            if (_view.Data.HasState(ScreenState.AvailableToSendSignal))
                _signals.Outgoing.LobbyClicked.Dispatch();
        }

        private void OnQuitClicked()
        {
            if (_view.Data.HasState(ScreenState.AvailableToSendSignal))
                _signals.Outgoing.QuitClicked.Dispatch();
        }
    }
}

using FlowIoC.BaseModule.Injectable.Attributes;
using FlowIoC.BaseModule.ViewsMediators.Mediator;
using FlowIoC.ScreenModule.Enums;
using FlowIoC.ScreenModule.Extensions;
using FlowIoC.ScreenModule.ViewsMediators.Screen;
using Modules.MatchModule.MatchScreenModule.Signals;

namespace Modules.MatchModule.MatchScreenModule.ViewsMediators
{
    public class MatchScreenMediator : IMediator
    {
        [Inject] private MatchScreenView _view { get; set; }
        [InjectSignal] private MatchScreenSignals _signals { get; set; }

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
            _view.ExitClicked += OnExitClicked;
            _view.ExitConfirmed += OnExitConfirmed;
            _signals.Incoming.ApplyScores.AddListener(_view.ShowScores);
            _signals.Incoming.ShowGameOver.AddListener(_view.ShowGameOver);
            _signals.Incoming.ApplyCameraSettings.AddListener(_view.ShowCameraSettings);
            _view.CameraSettingsEdited += OnCameraSettingsEdited;
        }

        private void OnScreenHidden(IScreenBody screen)
        {
            _view.ExitClicked -= OnExitClicked;
            _view.ExitConfirmed -= OnExitConfirmed;
            _signals.Incoming.ApplyScores.RemoveListener(_view.ShowScores);
            _signals.Incoming.ShowGameOver.RemoveListener(_view.ShowGameOver);
            _signals.Incoming.ApplyCameraSettings.RemoveListener(_view.ShowCameraSettings);
            _view.CameraSettingsEdited -= OnCameraSettingsEdited;
        }

        // Asking is always the answer to the button, so the question goes up with nothing decided.
        private void OnExitClicked()
        {
            if (_view.Data.HasState(ScreenState.AvailableToSendSignal))
                _view.ShowExitConfirm();
        }

        private void OnCameraSettingsEdited(float height, float followSpeed, float shake) =>
            _signals.Outgoing.CameraSettingsEdited.Dispatch(height, followSpeed, shake);

        private void OnExitConfirmed()
        {
            if (_view.Data.HasState(ScreenState.AvailableToSendSignal))
                _signals.Outgoing.ExitClicked.Dispatch();
        }
    }
}

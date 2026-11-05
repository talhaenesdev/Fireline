using System;
using FlowIoC.BaseModule.Controller;
using FlowIoC.BaseModule.Injectable.Attributes;
using FlowIoC.ConsoleModule;
using FlowIoC.ScreenModule.Service;
using Modules.MatchModule.MatchScreenModule.Models;
using Modules.MatchModule.MatchScreenModule.ViewsMediators;
using Modules.MatchModule.Shared.Data.ValueObjects;
using Modules.PlayerModule.Shared.Data.ValueObjects;
using Modules.LocalizationModule.Services;

namespace Modules.MatchModule.MatchScreenModule.Controllers
{
    internal class OpenMatchScreenCommand : Command
    {
        [Inject] private IScreenService _screenService { get; set; }
        [Inject] private ILocalizationService _localization { get; set; }
        [Inject] private IMatchScreenModel _model { get; set; }

        public override async void Execute()
        {
            Retain();

            try
            {
                Func<string, string> localize = _localization.Get;
                MatchScreenView screen = await _screenService.Open<MatchScreenView>().Show<MatchScreenView>();
                if (screen == null)
                {
                    FlowLogger.LogError("OpenMatchScreenCommand - the match screen did not open.");
                    Stop();
                    return;
                }

                screen.Localizer.Use(localize);
                screen.ShowScores(_model.Scores);
                if (_model.CameraSettings.HasValue)
                    screen.ShowCameraSettings(_model.CameraSettings.Value);
                Release();
            }
            catch (Exception exception)
            {
                FlowLogger.LogError($"OpenMatchScreenCommand threw while opening the screen: {exception}");
                Stop();
            }
        }
    }

    internal class RememberScoresCommand : Command
    {
        [Inject] private IMatchScreenModel _model { get; set; }

        [SignalParam] private ScoreEntryVO[] _scores { get; set; }

        public override void Execute() => _model.Scores = _scores;
    }

    internal class RememberCameraSettingsCommand : Command
    {
        [Inject] private IMatchScreenModel _model { get; set; }

        [SignalParam] private CameraSettingsVO _settings { get; set; }

        public override void Execute() => _model.CameraSettings = _settings;
    }
}

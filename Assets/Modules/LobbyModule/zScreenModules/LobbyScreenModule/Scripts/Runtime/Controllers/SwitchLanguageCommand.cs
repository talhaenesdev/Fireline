using FlowIoC.BaseModule.Controller;
using FlowIoC.BaseModule.Injectable.Attributes;
using FlowIoC.ScreenModule.Service;
using Modules.LobbyModule.LobbyScreenModule.ViewsMediators;
using Modules.LocalizationModule.Services;

namespace Modules.LobbyModule.LobbyScreenModule.Controllers
{
    /// <summary>The language button moves to the next language, and the open lobby is redrawn in it.</summary>
    internal class SwitchLanguageCommand : Command
    {
        [Inject] private ILocalizationService _localization { get; set; }
        [Inject] private IScreenService _screenService { get; set; }

        public override void Execute()
        {
            _localization.SetNextLanguage();

            if (!_screenService.TryGet.Screen(out LobbyScreenView screen))
                return;

            screen.Localizer.Use(_localization.Get);
            screen.ShowLanguage(_localization.Current);
        }
    }
}

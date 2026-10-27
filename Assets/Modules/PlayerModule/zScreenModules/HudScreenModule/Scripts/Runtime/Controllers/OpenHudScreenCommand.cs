using System;
using FlowIoC.BaseModule.Controller;
using FlowIoC.BaseModule.Injectable.Attributes;
using FlowIoC.ConsoleModule;
using FlowIoC.ScreenModule.Service;
using Modules.PlayerModule.HudScreenModule.Models;
using Modules.PlayerModule.HudScreenModule.ViewsMediators;

namespace Modules.PlayerModule.HudScreenModule.Controllers
{
    internal class OpenHudScreenCommand : Command
    {
        [Inject] private IScreenService _screenService { get; set; }
        [Inject] private IHudScreenModel _model { get; set; }

        public override async void Execute()
        {
            Retain();

            try
            {
                HudScreenView screen = await _screenService.Open<HudScreenView>().Show<HudScreenView>();
                if (screen == null)
                {
                    FlowLogger.LogError("OpenHudScreenCommand - the HUD did not open.");
                    Stop();
                    return;
                }

                screen.ShowHealth(_model.Health, _model.MaxHealth);
                screen.ShowAmmo(_model.Ammo, _model.MagazineSize);
                Release();
            }
            catch (Exception exception)
            {
                FlowLogger.LogError($"OpenHudScreenCommand threw while opening the HUD: {exception}");
                Stop();
            }
        }
    }
}

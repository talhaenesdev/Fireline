using System;
using FlowIoC.BaseModule.Controller;
using FlowIoC.BaseModule.Injectable.Attributes;
using FlowIoC.ConsoleModule;
using FlowIoC.ScreenModule.Service;
using Modules.PlayerModule.HudScreenModule.Models;
using Modules.PlayerModule.HudScreenModule.ViewsMediators;
using UnityEngine;
using Modules.LocalizationModule.Services;

namespace Modules.PlayerModule.HudScreenModule.Controllers
{
    internal class OpenHudScreenCommand : Command
    {
        [Inject] private IScreenService _screenService { get; set; }
        [Inject] private ILocalizationService _localization { get; set; }
        [Inject] private IHudScreenModel _model { get; set; }

        public override async void Execute()
        {
            Retain();

            try
            {
                Func<string, string> localize = _localization.Get;
                HudScreenView screen = await _screenService.Open<HudScreenView>().Show<HudScreenView>();
                if (screen == null)
                {
                    FlowLogger.LogError("OpenHudScreenCommand - the HUD did not open.");
                    Stop();
                    return;
                }

                screen.Localizer.Use(localize);
                screen.ShowHealth(_model.Health, _model.MaxHealth);
                screen.ShowAmmo(_model.Ammo, _model.MagazineSize, _model.Reserve);
                screen.ShowShield(_model.Shield);
                if (_model.HasWallet)
                    screen.ShowWallet(_model.Money, _model.Carried, _model.Materials);
                screen.ShowWave(_model.Wave, Mathf.Max(0f, _model.WaveStartTime - Time.time), _model.ZombiesLeft);
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

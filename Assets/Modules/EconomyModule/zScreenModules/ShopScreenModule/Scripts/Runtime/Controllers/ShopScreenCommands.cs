using System;
using FlowIoC.BaseModule.Controller;
using FlowIoC.BaseModule.Injectable.Attributes;
using FlowIoC.ConsoleModule;
using FlowIoC.ScreenModule.Service;
using Modules.EconomyModule.Shared.Data.ValueObjects;
using Modules.EconomyModule.ShopScreenModule.Models;
using Modules.EconomyModule.ShopScreenModule.ViewsMediators;
using Modules.LocalizationModule.Services;

namespace Modules.EconomyModule.ShopScreenModule.Controllers
{
    internal class RememberItemsCommand : Command
    {
        [Inject] private IShopScreenModel _model { get; set; }

        [SignalParam] private ShopItemCVO[] _items { get; set; }

        public override void Execute() => _model.Items = _items;
    }

    internal class RememberMoneyCommand : Command
    {
        [Inject] private IShopScreenModel _model { get; set; }

        [SignalParam(0)] private int _money { get; set; }
        [SignalParam(2)] private int _materials { get; set; }

        public override void Execute()
        {
            _model.Money = _money;
            _model.Materials = _materials;
        }
    }

    internal class OpenShopScreenCommand : Command
    {
        [Inject] private IScreenService _screenService { get; set; }
        [Inject] private ILocalizationService _localization { get; set; }
        [Inject] private IShopScreenModel _model { get; set; }

        public override async void Execute()
        {
            Retain();

            try
            {
                Func<string, string> localize = _localization.Get;
                ShopScreenView screen = await _screenService.Open<ShopScreenView>().Show<ShopScreenView>();
                if (screen == null)
                {
                    FlowLogger.LogError("OpenShopScreenCommand - the shop did not open.");
                    Stop();
                    return;
                }

                screen.Localizer.Use(localize);
                screen.ShowItems(_model.Items);
                screen.ShowWallet(_model.Money, 0, _model.Materials);
                Release();
            }
            catch (Exception exception)
            {
                FlowLogger.LogError($"OpenShopScreenCommand threw while opening the shop: {exception}");
                Stop();
            }
        }
    }

    internal class CloseShopScreenCommand : Command
    {
        [Inject] private IScreenService _screenService { get; set; }

        public override void Execute() => _screenService.Hide.Screen<ShopScreenView>();
    }
}

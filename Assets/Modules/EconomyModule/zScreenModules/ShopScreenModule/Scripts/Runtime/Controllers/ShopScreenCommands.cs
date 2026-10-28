using System;
using FlowIoC.BaseModule.Controller;
using FlowIoC.BaseModule.Injectable.Attributes;
using FlowIoC.ConsoleModule;
using FlowIoC.ScreenModule.Service;
using Modules.EconomyModule.Shared.Data.ValueObjects;
using Modules.EconomyModule.ShopScreenModule.Models;
using Modules.EconomyModule.ShopScreenModule.ViewsMediators;

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

        [SignalParam] private int _money { get; set; }

        public override void Execute() => _model.Money = _money;
    }

    internal class OpenShopScreenCommand : Command
    {
        [Inject] private IScreenService _screenService { get; set; }
        [Inject] private IShopScreenModel _model { get; set; }

        public override async void Execute()
        {
            Retain();

            try
            {
                ShopScreenView screen = await _screenService.Open<ShopScreenView>().Show<ShopScreenView>();
                if (screen == null)
                {
                    FlowLogger.LogError("OpenShopScreenCommand - the shop did not open.");
                    Stop();
                    return;
                }

                screen.ShowItems(_model.Items);
                screen.ShowMoney(_model.Money);
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

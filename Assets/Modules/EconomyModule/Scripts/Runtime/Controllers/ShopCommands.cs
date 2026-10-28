using System.Linq;
using FlowIoC.BaseModule.Controller;
using FlowIoC.BaseModule.Injectable.Attributes;
using Modules.EconomyModule.Enums;
using Modules.EconomyModule.Models;
using Modules.EconomyModule.Shared.Data.ValueObjects;
using Modules.EconomyModule.Shared.Enums;
using Modules.EconomyModule.Signals;
using Unity.Netcode;

namespace Modules.EconomyModule.Controllers
{
    /// <summary>The local player stepping into or out of the shop zone opens or closes the shop.</summary>
    internal class CheckShopZoneCommand : Command
    {
        [Inject] private IEconomyModel _model { get; set; }
        [InjectSignal] private EconomySignals _signals { get; set; }

        public override void Execute()
        {
            NetworkManager manager = NetworkManager.Singleton;
            if (_model.Board == null || !_model.Board.IsSpawned || manager == null || !manager.IsClient)
                return;

            bool isInShop = _model.Zones.Any(zone => zone.Kind == EconomyZoneKind.Shop && EconomyPlayers.IsWithinLocal(zone.Position, zone.Radius));
            if (isInShop == _model.IsInShop)
                return;

            _model.IsInShop = isInShop;
            if (isInShop)
                _signals.Outgoing.ShopEntered.Dispatch(_model.Board.Shop.Items.ToArray());
            else
                _signals.Outgoing.ShopLeft.Dispatch();
        }
    }

    /// <summary>The local player's wallet, from the replicated board, for the HUD and the shop.</summary>
    internal class PublishLocalWalletCommand : Command
    {
        [Inject] private IEconomyModel _model { get; set; }
        [InjectSignal] private EconomySignals _signals { get; set; }

        public override void Execute()
        {
            NetworkManager manager = NetworkManager.Singleton;
            if (_model.Board == null || manager == null || !manager.IsClient)
                return;

            _model.Board.TryGetWallet(manager.LocalClientId, out int money, out int carried);
            _signals.Outgoing.LocalWalletChanged.Dispatch(money, carried);
        }
    }

    /// <summary>The local player asks the server for an item; the server decides.</summary>
    internal class RequestBuyCommand : Command
    {
        [Inject] private IEconomyModel _model { get; set; }

        [SignalParam] private int _item { get; set; }

        public override void Execute()
        {
            if (_model.Board != null && _model.Board.IsSpawned)
                _model.Board.RequestBuy(_item);
        }
    }

    /// <summary>
    /// Server: a player with the money pays for the item. Health and shields are the server's to
    /// give; a faster trigger is the buyer's own weapon, so the buyer is told.
    /// </summary>
    internal class SellItemCommand : Command
    {
        [Inject] private IEconomyModel _model { get; set; }
        [InjectSignal] private EconomySignals _signals { get; set; }

        [SignalParam] private ulong _clientId { get; set; }
        [SignalParam] private int _item { get; set; }

        public override void Execute()
        {
            var items = _model.Board.Shop.Items;
            if (_item < 0 || _item >= items.Count)
                return;

            ShopItemCVO item = items[_item];
            int money = EconomyWallets.MoneyOf(_model, _clientId);
            if (money < item.Price || !EconomyPlayers.IsInShop(_model, _clientId))
                return;

            _model.Money[_clientId] = money - item.Price;
            EconomyWallets.Publish(_model, _clientId);

            switch (item.Type)
            {
                case ShopItemType.Heal:
                    _signals.Outgoing.HealPurchased.Dispatch(_clientId, item.Amount);
                    break;
                case ShopItemType.Shield:
                    _signals.Outgoing.ShieldPurchased.Dispatch(_clientId, item.Amount);
                    break;
                case ShopItemType.FireRate:
                    _model.Board.ConfirmPurchaseServer(_clientId, _item);
                    break;
                case ShopItemType.Turret:
                    _signals.Outgoing.TurretPurchased.Dispatch(_clientId);
                    break;
            }
        }
    }

    /// <summary>Buyer's process: what the server sold is applied where it lives.</summary>
    internal class ApplyPurchaseCommand : Command
    {
        [Inject] private IEconomyModel _model { get; set; }
        [InjectSignal] private EconomySignals _signals { get; set; }

        [SignalParam] private int _item { get; set; }

        public override void Execute()
        {
            ShopItemCVO item = _model.Board.Shop.Items[_item];
            if (item.Type == ShopItemType.FireRate)
                _signals.Outgoing.FireRatePurchased.Dispatch(item.Amount);
        }
    }
}

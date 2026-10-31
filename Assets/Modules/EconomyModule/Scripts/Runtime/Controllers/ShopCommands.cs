using System.Linq;
using FlowIoC.BaseModule.Controller;
using FlowIoC.BaseModule.Injectable.Attributes;
using System.Collections.Generic;
using Modules.EconomyModule.Entities;
using Modules.EconomyModule.Enums;
using Modules.EconomyModule.Models;
using Modules.EconomyModule.Shared.Data.ValueObjects;
using Modules.EconomyModule.Shared.Enums;
using Modules.EconomyModule.Signals;
using Unity.Netcode;

namespace Modules.EconomyModule.Controllers
{
    /// <summary>The local player stepping into or out of a shop zone or a perk machine opens or closes its stock.</summary>
    internal class CheckShopZoneCommand : Command
    {
        [Inject] private IEconomyModel _model { get; set; }
        [InjectSignal] private EconomySignals _signals { get; set; }

        public override void Execute()
        {
            NetworkManager manager = NetworkManager.Singleton;
            if (_model.Board == null || !_model.Board.IsSpawned || manager == null || !manager.IsClient)
                return;

            IEconomyZone zone = _model.Zones.FirstOrDefault(candidate => candidate.Kind != EconomyZoneKind.Sell
                                                                         && !(candidate.Kind == EconomyZoneKind.Door && _model.Board.IsDoorOpen(candidate.Id))
                                                                         && EconomyPlayers.IsWithinLocal(candidate.Position, candidate.Radius));
            int zoneId = zone?.Id ?? -1;
            if (zoneId == _model.ActiveZoneId)
                return;

            if (_model.ActiveZoneId >= 0)
                _signals.Outgoing.ShopLeft.Dispatch();

            _model.ActiveZoneId = zoneId;
            _model.IsInShop = zone != null;
            if (zone != null)
                _signals.Outgoing.ShopEntered.Dispatch(EconomyShops.StockOf(_model, zone).Items.ToArray());
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
            if (_model.Board != null && _model.Board.IsSpawned && _model.ActiveZoneId >= 0)
                _model.Board.RequestBuy(_model.ActiveZoneId, _item);
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
        [SignalParam(0)] private int _zone { get; set; }
        [SignalParam(1)] private int _item { get; set; }

        public override void Execute()
        {
            IEconomyZone zone = _model.Zones.FirstOrDefault(candidate => candidate.Id == _zone && candidate.Kind != EconomyZoneKind.Sell);
            if (zone == null || !EconomyPlayers.IsWithin(_clientId, zone.Position, zone.Radius))
                return;

            var items = EconomyShops.StockOf(_model, zone).Items;
            if (_item < 0 || _item >= items.Count)
                return;

            ShopItemCVO item = items[_item];
            int money = EconomyWallets.MoneyOf(_model, _clientId);
            bool isOnce = item.Type == ShopItemType.Perk || item.Type == ShopItemType.ReloadPerk;
            if (!_model.OwnedOnce.TryGetValue(_clientId, out var owned))
                _model.OwnedOnce[_clientId] = owned = new HashSet<(int zone, int item)>();

            if (money < item.Price || (isOnce && owned.Contains((_zone, _item))))
                return;

            if (item.Type == ShopItemType.Door && _model.Board.IsDoorOpen(_zone))
                return;

            if (isOnce)
                owned.Add((_zone, _item));

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
                case ShopItemType.ReloadPerk:
                    _model.Board.ConfirmPurchaseServer(_clientId, _zone, _item);
                    break;
                case ShopItemType.Door:
                    _model.Board.OpenDoorServer(_zone);
                    break;
                case ShopItemType.Perk:
                    _signals.Outgoing.PerkPurchased.Dispatch(_clientId, item.Index, item.Amount);
                    break;
                case ShopItemType.Turret:
                    _signals.Outgoing.TurretPurchased.Dispatch(_clientId);
                    break;
                case ShopItemType.Weapon:
                    _signals.Outgoing.WeaponPurchased.Dispatch(_clientId, item.Index);
                    break;
                case ShopItemType.TurretUpgrade:
                    _signals.Outgoing.TurretUpgradePurchased.Dispatch(_clientId);
                    break;
                case ShopItemType.Barricade:
                    _signals.Outgoing.BarricadePurchased.Dispatch(_clientId);
                    break;
                case ShopItemType.Wall:
                    _signals.Outgoing.WallPurchased.Dispatch(_clientId);
                    break;
                case ShopItemType.Barracks:
                    _signals.Outgoing.BarracksPurchased.Dispatch(_clientId);
                    break;
            }
        }
    }

    /// <summary>Buyer's process: what the server sold is applied where it lives.</summary>
    internal class ApplyPurchaseCommand : Command
    {
        [Inject] private IEconomyModel _model { get; set; }
        [InjectSignal] private EconomySignals _signals { get; set; }

        [SignalParam(0)] private int _zone { get; set; }
        [SignalParam(1)] private int _item { get; set; }

        public override void Execute()
        {
            IEconomyZone zone = _model.Zones.FirstOrDefault(candidate => candidate.Id == _zone);
            if (zone == null)
                return;

            var items = EconomyShops.StockOf(_model, zone).Items;
            if (_item < 0 || _item >= items.Count)
                return;

            ShopItemCVO item = items[_item];
            if (item.Type == ShopItemType.FireRate)
                _signals.Outgoing.FireRatePurchased.Dispatch(item.Amount);
            else if (item.Type == ShopItemType.ReloadPerk)
                _signals.Outgoing.ReloadPerkPurchased.Dispatch(item.Amount);
        }
    }

    /// <summary>A zone's stock: its own, or the board's shop.</summary>
    internal static class EconomyShops
    {
        public static Data.UnityObjects.CD_Shop StockOf(IEconomyModel model, IEconomyZone zone) => zone.Shop != null ? zone.Shop : model.Board.Shop;
    }
}

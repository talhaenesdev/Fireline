using System.Collections.Generic;
using FlowIoC.BaseModule.Controller;
using FlowIoC.BaseModule.Injectable.Attributes;
using Modules.EconomyModule.Data.UnityObjects;
using Modules.EconomyModule.Enums;
using Modules.EconomyModule.Models;
using Unity.Netcode;
using UnityEngine;

namespace Modules.EconomyModule.Controllers
{
    /// <summary>A fallen zombie leaves a piece of loot, by chance, picked by the items' weights.</summary>
    internal class DropLootCommand : Command
    {
        [Inject] private IEconomyModel _model { get; set; }

        [SignalParam] private Vector3 _position { get; set; }

        public override void Execute()
        {
            CD_Loot loot = _model.Board.Loot;
            if (loot == null || loot.Items.Count == 0 || Random.value > loot.DropChance)
                return;

            float total = 0f;
            foreach (var item in loot.Items)
                total += item.Weight;

            float roll = Random.value * total;
            int chosen = 0;
            for (int i = 0; i < loot.Items.Count; i++)
            {
                roll -= loot.Items[i].Weight;
                if (roll > 0f)
                    continue;

                chosen = i;
                break;
            }

            Vector3 ground = new(_position.x, 0f, _position.z);
            _model.Board.AddLootServer(_model.NextLootId++, chosen, ground);
        }
    }

    /// <summary>A living player who walks over loot puts it in their bag.</summary>
    internal class CollectLootCommand : Command
    {
        [Inject] private IEconomyModel _model { get; set; }

        public override void Execute()
        {
            if (_model.Board.LootCount == 0)
                return;

            float radiusSqr = _model.Board.Loot.PickupRadius * _model.Board.Loot.PickupRadius;
            var taken = new List<int>();

            for (int i = 0; i < _model.Board.LootCount; i++)
            {
                _model.Board.GetLoot(i, out int id, out int item, out Vector3 position);
                if (!EconomyPlayers.TryNearest(position, radiusSqr, out ulong clientId))
                    continue;

                Bag(clientId).Add(item);
                taken.Add(id);
                EconomyWallets.Publish(_model, clientId);
            }

            foreach (int id in taken)
                _model.Board.RemoveLootServer(id);
        }

        private List<int> Bag(ulong clientId)
        {
            if (!_model.Carried.TryGetValue(clientId, out List<int> bag))
                _model.Carried[clientId] = bag = new List<int>();

            return bag;
        }
    }

    /// <summary>A player carrying loot into the sell zone sells all of it.</summary>
    internal class SellLootCommand : Command
    {
        [Inject] private IEconomyModel _model { get; set; }

        public override void Execute()
        {
            foreach (var zone in _model.Zones)
            {
                if (zone.Kind != EconomyZoneKind.Sell)
                    continue;

                foreach (KeyValuePair<ulong, List<int>> bag in _model.Carried)
                {
                    if (bag.Value.Count == 0 || !EconomyPlayers.IsWithin(bag.Key, zone.Position, zone.Radius))
                        continue;

                    int earned = 0;
                    foreach (int item in bag.Value)
                        earned += _model.Board.Loot.Items[item].SellPrice;

                    bag.Value.Clear();
                    _model.Money[bag.Key] = EconomyWallets.MoneyOf(_model, bag.Key) + earned;
                    EconomyWallets.Publish(_model, bag.Key);
                }
            }
        }
    }

    /// <summary>The server's wallets as the board shows them.</summary>
    internal static class EconomyWallets
    {
        public static int MoneyOf(IEconomyModel model, ulong clientId) =>
            model.Money.TryGetValue(clientId, out int money) ? money : 0;

        public static void Publish(IEconomyModel model, ulong clientId)
        {
            int carried = model.Carried.TryGetValue(clientId, out List<int> bag) ? bag.Count : 0;
            model.Board.SetWalletServer(clientId, MoneyOf(model, clientId), carried);
        }
    }

    /// <summary>Where the players stand, by their player objects.</summary>
    internal static class EconomyPlayers
    {
        public static bool TryNearest(Vector3 point, float radiusSqr, out ulong clientId)
        {
            clientId = 0;
            float best = radiusSqr;
            bool found = false;

            foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
            {
                if (client.PlayerObject == null || !client.PlayerObject.IsSpawned)
                    continue;

                Vector3 offset = client.PlayerObject.transform.position - point;
                offset.y = 0f;
                if (offset.sqrMagnitude > best)
                    continue;

                best = offset.sqrMagnitude;
                clientId = client.ClientId;
                found = true;
            }

            return found;
        }

        /// <summary>Client side: only the local player's own object is known by id there.</summary>
        public static bool IsWithinLocal(Vector3 point, float radius)
        {
            NetworkObject player = NetworkManager.Singleton.LocalClient?.PlayerObject;
            if (player == null || !player.IsSpawned)
                return false;

            Vector3 offset = player.transform.position - point;
            offset.y = 0f;
            return offset.sqrMagnitude <= radius * radius;
        }

        public static bool IsInShop(IEconomyModel model, ulong clientId)
        {
            foreach (var zone in model.Zones)
            {
                if (zone.Kind == EconomyZoneKind.Shop && IsWithin(clientId, zone.Position, zone.Radius))
                    return true;
            }

            return false;
        }

        public static bool IsWithin(ulong clientId, Vector3 point, float radius)
        {
            if (!NetworkManager.Singleton.ConnectedClients.TryGetValue(clientId, out NetworkClient client)
                || client.PlayerObject == null || !client.PlayerObject.IsSpawned)
                return false;

            Vector3 offset = client.PlayerObject.transform.position - point;
            offset.y = 0f;
            return offset.sqrMagnitude <= radius * radius;
        }
    }
}

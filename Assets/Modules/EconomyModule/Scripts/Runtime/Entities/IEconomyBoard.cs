using Modules.EconomyModule.Data.UnityObjects;
using UnityEngine;

namespace Modules.EconomyModule.Entities
{
    /// <summary>
    /// The match's replicated economy: every player's wallet and the loot lying on the ground. The
    /// server writes it; every client reads it, and asks the server to buy through it.
    /// </summary>
    internal interface IEconomyBoard
    {
        CD_Loot Loot { get; }
        CD_Shop Shop { get; }
        bool IsSpawned { get; }
        bool IsServer { get; }
        int LootCount { get; }

        void GetLoot(int index, out int id, out int item, out Vector3 position);
        bool TryGetWallet(ulong clientId, out int money, out int carried);

        void AddLootServer(int id, int item, Vector3 position);
        void RemoveLootServer(int id);
        void SetWalletServer(ulong clientId, int money, int carried);

        /// <summary>Client: asks the server for one item of the shop.</summary>
        void RequestBuy(int item);

        /// <summary>Server: tells the buyer what it bought, for what only the buyer's process applies.</summary>
        void ConfirmPurchaseServer(ulong clientId, int item);
    }
}

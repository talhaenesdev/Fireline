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
        /// <summary>The pool the board draws what it shows from, handed over once it registers.</summary>
        void Configure(FlowIoC.PoolModule.Services.IPoolService pool);

        CD_Loot Loot { get; }
        CD_Shop Shop { get; }
        CD_Progression Progression { get; }
        bool IsSpawned { get; }
        bool IsServer { get; }
        int LootCount { get; }

        void GetLoot(int index, out int id, out int item, out Vector3 position);
        bool TryGetWallet(ulong clientId, out int money, out int carried, out int materials);

        void AddLootServer(int id, int item, Vector3 position);
        void RemoveLootServer(int id);
        void SetWalletServer(ulong clientId, int money, int carried, int materials);

        /// <summary>Materials: how fast they are gathered, and what a player starts with. None on a map without them.</summary>
        CD_Resources Resources { get; }

        /// <summary>A player's experience this match.</summary>
        bool TryGetXp(ulong clientId, out int xp);
        void SetXpServer(ulong clientId, int xp);

        bool IsDoorOpen(int zone);
        void OpenDoorServer(int zone);

        /// <summary>Client: tells the server the local player's level, for the money it starts with.</summary>
        void ReportLevel(int level);

        /// <summary>Client: asks the server for one item of a zone's stock.</summary>
        void RequestBuy(int zone, int item);

        /// <summary>Server: tells the buyer what it bought, for what only the buyer's process applies.</summary>
        void ConfirmPurchaseServer(ulong clientId, int zone, int item);
    }
}

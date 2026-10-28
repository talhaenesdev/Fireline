using System.Collections.Generic;
using Modules.EconomyModule.Entities;

namespace Modules.EconomyModule.Models
{
    internal interface IEconomyModel
    {
        IEconomyBoard Board { get; set; }
        List<IEconomyZone> Zones { get; }

        /// <summary>Server: the loot each player carries, by item, until it is sold.</summary>
        Dictionary<ulong, List<int>> Carried { get; }

        /// <summary>Server: each player's money.</summary>
        Dictionary<ulong, int> Money { get; }

        int NextLootId { get; set; }

        /// <summary>The local player stands in the shop zone.</summary>
        bool IsInShop { get; set; }

        bool IsTicking { get; set; }
        bool IsHooked { get; set; }
    }

    internal class EconomyModel : IEconomyModel
    {
        public IEconomyBoard Board { get; set; }
        public List<IEconomyZone> Zones { get; } = new();
        public Dictionary<ulong, List<int>> Carried { get; } = new();
        public Dictionary<ulong, int> Money { get; } = new();
        public int NextLootId { get; set; }
        public bool IsInShop { get; set; }
        public bool IsTicking { get; set; }
        public bool IsHooked { get; set; }
    }
}

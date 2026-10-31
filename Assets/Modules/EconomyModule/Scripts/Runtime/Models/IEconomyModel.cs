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

        /// <summary>Server: each player's experience this match, and those already paid for their level.</summary>
        Dictionary<ulong, int> Xp { get; }
        HashSet<ulong> PaidForLevel { get; }

        /// <summary>The local player's experience this match as last saved into the total.</summary>
        int SavedMatchXp { get; set; }
        bool HasReportedLevel { get; set; }

        /// <summary>The local player stands in the shop zone.</summary>
        bool IsInShop { get; set; }

        /// <summary>The zone whose stock the local player sees; below zero when none.</summary>
        int ActiveZoneId { get; set; }

        /// <summary>Server: the once-only items each player owns, as zone and item.</summary>
        Dictionary<ulong, HashSet<(int zone, int item)>> OwnedOnce { get; }

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
        public Dictionary<ulong, int> Xp { get; } = new();
        public HashSet<ulong> PaidForLevel { get; } = new();
        public int SavedMatchXp { get; set; }
        public bool HasReportedLevel { get; set; }
        public bool IsInShop { get; set; }
        public int ActiveZoneId { get; set; } = -1;
        public Dictionary<ulong, HashSet<(int zone, int item)>> OwnedOnce { get; } = new();
        public bool IsTicking { get; set; }
        public bool IsHooked { get; set; }
    }
}

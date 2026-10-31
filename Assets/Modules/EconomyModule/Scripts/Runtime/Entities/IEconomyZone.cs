using Modules.EconomyModule.Enums;
using UnityEngine;

namespace Modules.EconomyModule.Entities
{
    /// <summary>A place on the map: the sell zone takes a player's loot, a shop zone or a perk machine opens its stock.</summary>
    internal interface IEconomyZone
    {
        /// <summary>The same on every process, so a purchase names the zone it was made at.</summary>
        int Id { get; }
        EconomyZoneKind Kind { get; }

        /// <summary>What it sells; none means the board's shop.</summary>
        Data.UnityObjects.CD_Shop Shop { get; }
        Vector3 Position { get; }

        /// <summary>A door: takes its blocker away.</summary>
        void ShowOpen(bool isOpen);
        float Radius { get; }
    }
}

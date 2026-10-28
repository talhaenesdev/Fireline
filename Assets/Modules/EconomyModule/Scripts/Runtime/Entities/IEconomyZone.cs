using Modules.EconomyModule.Enums;
using UnityEngine;

namespace Modules.EconomyModule.Entities
{
    /// <summary>A place on the map: the sell zone takes a player's loot, the shop zone opens the shop.</summary>
    internal interface IEconomyZone
    {
        EconomyZoneKind Kind { get; }
        Vector3 Position { get; }
        float Radius { get; }
    }
}

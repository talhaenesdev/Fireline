using Modules.PlayerModule.Shared.Enums;
using UnityEngine;

namespace Modules.PlayerModule.Shared.Data.ValueObjects
{
    /// <summary>A point in the world the HUD may point at, and what it is.</summary>
    public readonly struct MarkerVO
    {
        public readonly Vector3 Position;
        public readonly MarkerKind Kind;

        public MarkerVO(Vector3 position, MarkerKind kind)
        {
            Position = position;
            Kind = kind;
        }
    }
}

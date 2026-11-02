using Modules.PlayerModule.Shared.Enums;
using UnityEngine;

namespace Modules.PlayerModule.Shared.Data.ValueObjects
{
    /// <summary>A point in the world the HUD may point at, and what it is.</summary>
    public readonly struct MarkerVO
    {
        public readonly Vector3 Position;
        public readonly MarkerKind Kind;

        /// <summary>A name to show beside the marker; empty for none.</summary>
        public readonly string Label;

        public MarkerVO(Vector3 position, MarkerKind kind, string label = "")
        {
            Position = position;
            Kind = kind;
            Label = label ?? string.Empty;
        }
    }
}

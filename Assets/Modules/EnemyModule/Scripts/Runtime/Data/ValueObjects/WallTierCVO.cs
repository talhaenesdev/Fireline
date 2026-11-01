using System;
using UnityEngine;

namespace Modules.EnemyModule.Data.ValueObjects
{
    /// <summary>One tier of the walls: what a reinforcement buys, and how the walls look at it.</summary>
    [Serializable]
    public class WallTierCVO
    {
        public string Name = "Tahta";
        public int Health = 600;
        public Color Color = new(0.6f, 0.45f, 0.3f, 1f);
    }
}

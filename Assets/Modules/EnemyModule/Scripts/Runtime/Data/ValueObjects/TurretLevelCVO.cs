using System;

namespace Modules.EnemyModule.Data.ValueObjects
{
    /// <summary>One level of a turret: what an upgrade buys.</summary>
    [Serializable]
    public class TurretLevelCVO
    {
        public float Range = 12f;
        public float FireInterval = 0.45f;
        public int Damage = 10;
        public int Health = 150;
    }
}

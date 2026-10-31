using System;

namespace Modules.EnemyModule.Data.ValueObjects
{
    /// <summary>One difficulty of a zombie match: how many zombies, how tough, and how hard they hit.</summary>
    [Serializable]
    public class ZombieDifficultyCVO
    {
        public string Name = "Normal";
        public float CountMultiplier = 1f;
        public float HealthMultiplier = 1f;
        public float DamageMultiplier = 1f;

        public ZombieTypeCVO Apply(ZombieTypeCVO type)
        {
            if (Math.Abs(HealthMultiplier - 1f) < 0.001f && Math.Abs(DamageMultiplier - 1f) < 0.001f)
                return type;

            ZombieTypeCVO scaled = type.Clone();
            scaled.Health = Math.Max(1, (int) Math.Round(type.Health * HealthMultiplier));
            scaled.Damage = (int) Math.Round(type.Damage * DamageMultiplier);
            scaled.SpitDamage = (int) Math.Round(type.SpitDamage * DamageMultiplier);
            scaled.AreaDamage = (int) Math.Round(type.AreaDamage * DamageMultiplier);
            return scaled;
        }
    }
}

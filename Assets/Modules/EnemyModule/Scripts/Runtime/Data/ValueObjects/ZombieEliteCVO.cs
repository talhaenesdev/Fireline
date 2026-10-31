using System;
using UnityEngine;

namespace Modules.EnemyModule.Data.ValueObjects
{
    /// <summary>What turns any kind of zombie into its elite: tougher, harder-hitting, bigger, and worth more.</summary>
    [Serializable]
    public class ZombieEliteCVO
    {
        public float HealthMultiplier = 2.5f;
        public float DamageMultiplier = 1.5f;
        public float SpeedMultiplier = 1.15f;
        public float ScaleMultiplier = 1.2f;
        public Color Tint = new(1f, 0.35f, 0.2f, 1f);
        public int LootRolls = 3;

        public ZombieTypeCVO Apply(ZombieTypeCVO type)
        {
            ZombieTypeCVO elite = type.Clone();
            elite.IsElite = true;
            elite.Health = Mathf.RoundToInt(type.Health * HealthMultiplier);
            elite.Damage = Mathf.RoundToInt(type.Damage * DamageMultiplier);
            elite.SpitDamage = Mathf.RoundToInt(type.SpitDamage * DamageMultiplier);
            elite.AreaDamage = Mathf.RoundToInt(type.AreaDamage * DamageMultiplier);
            elite.MoveSpeed = type.MoveSpeed * SpeedMultiplier;
            elite.Scale = type.Scale * ScaleMultiplier;
            elite.Tint = Tint;
            elite.LootRolls = Mathf.Max(type.LootRolls, LootRolls);
            elite.Name = "Elit " + type.Name;
            return elite;
        }
    }
}

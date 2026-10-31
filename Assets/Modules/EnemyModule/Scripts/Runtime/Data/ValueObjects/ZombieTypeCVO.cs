using System;
using Modules.EnemyModule.Enums;
using UnityEngine;

namespace Modules.EnemyModule.Data.ValueObjects
{
    /// <summary>One kind of zombie: how tough, fast and dangerous it is, how it fights, and how it looks.</summary>
    [Serializable]
    public class ZombieTypeCVO
    {
        [Tooltip("The name a wave uses to ask for this kind; a boss also shows it on its health bar.")]
        public string Name = "Walker";
        public ZombieBehaviour Behaviour = ZombieBehaviour.Melee;
        [Tooltip("A boss shows its health across the top of every player's screen.")]
        public bool IsBoss;

        [Header("Body")]
        public int Health = 30;
        public float MoveSpeed = 2.5f;
        [Tooltip("How far a hit pushes it back; a heavy zombie barely moves.")]
        public float HitKnockback = 0.4f;
        [Tooltip("Share of the damage taken off a hit that comes at its front, 0 to 1.")]
        [Range(0f, 1f)] public float FrontArmor;
        [Tooltip("How wide its front is, in degrees.")]
        public float ArmorAngle = 120f;
        [Tooltip("Its size against the prefab's.")]
        public float Scale = 1f;
        [Tooltip("Multiplies the model's colours, so each kind reads at a glance.")]
        public Color Tint = new(0.45f, 0.85f, 0.3f, 1f);
        [Tooltip("How many times the loot table is rolled when it falls.")]
        public int LootRolls = 1;

        [Header("Bite")]
        public int Damage = 10;
        public float AttackRange = 1.3f;
        public float AttackCooldown = 1f;

        [Header("Spitter: keeps this far away and spits")]
        public float PreferredRange = 9f;
        public float SpitRange = 13f;
        public float SpitSpeed = 9f;
        public int SpitDamage = 8;
        public float SpitCooldown = 2.2f;

        [Header("Exploder and Slammer: the blast around it")]
        public float AreaRadius = 3.5f;
        public int AreaDamage = 30;
        [Tooltip("Slammer: the rest between slams.")]
        public float AreaCooldown = 5f;

        /// <summary>Set on the copy an elite spawns with.</summary>
        [NonSerialized] public bool IsElite;

        public ZombieTypeCVO Clone() => (ZombieTypeCVO) MemberwiseClone();
    }
}

using System.Collections.Generic;
using Modules.EnemyModule.Data.ValueObjects;
using UnityEngine;

namespace Modules.EnemyModule.Data.UnityObjects
{
    /// <summary>
    /// A wall a player buys, and the walls the base starts with: how close a zombie must come before it
    /// is in the way, and the tiers a reinforcement takes every wall through. Element 0 is the tier the
    /// match starts on.
    /// </summary>
    [CreateAssetMenu(fileName = "CD_Wall", menuName = "Fireline/Enemy/CD_Wall")]
    public class CD_Wall : ScriptableObject
    {
        [Tooltip("The health of a wall when no tiers are set.")]
        [SerializeField] private int _health = 600;
        [Tooltip("A zombie this close to a wall stops and breaks it before going on.")]
        [SerializeField] private float _blockRadius = 2f;
        [SerializeField] private float _maxPlaceDistance = 8f;
        [SerializeField] private List<WallTierCVO> _tiers = new();

        public int Health => Tier(0).Health;
        public float BlockRadius => _blockRadius;
        public float MaxPlaceDistance => _maxPlaceDistance;
        public int TierCount => Mathf.Max(1, _tiers.Count);

        public WallTierCVO Tier(int tier) => _tiers.Count == 0
            ? new WallTierCVO { Health = _health, Color = new Color(0f, 0f, 0f, 0f) }
            : _tiers[Mathf.Clamp(tier, 0, _tiers.Count - 1)];
    }
}

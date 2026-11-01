using UnityEngine;

namespace Modules.EnemyModule.Data.UnityObjects
{
    /// <summary>Razor wire a player buys: zombies on it are slowed and hurt, and every hit wears it down.</summary>
    [CreateAssetMenu(fileName = "CD_SpikeTrap", menuName = "Fireline/Enemy/CD_SpikeTrap")]
    public class CD_SpikeTrap : ScriptableObject
    {
        [Tooltip("How much wear it takes before it is gone.")]
        [SerializeField] private int _health = 300;
        [SerializeField] private float _maxPlaceDistance = 6f;
        [SerializeField] private float _radius = 1.8f;
        [Tooltip("A zombie on the wire moves at this share of its speed.")]
        [Range(0f, 1f)] [SerializeField] private float _slowFactor = 0.4f;
        [SerializeField] private int _damage = 6;
        [SerializeField] private float _damageInterval = 0.5f;
        [Tooltip("Wear taken for every zombie it hurts.")]
        [SerializeField] private int _wearPerHit = 4;

        public int Health => _health;
        public float MaxPlaceDistance => _maxPlaceDistance;
        public float Radius => _radius;
        public float SlowFactor => _slowFactor;
        public int Damage => _damage;
        public float DamageInterval => _damageInterval;
        public int WearPerHit => _wearPerHit;
    }
}

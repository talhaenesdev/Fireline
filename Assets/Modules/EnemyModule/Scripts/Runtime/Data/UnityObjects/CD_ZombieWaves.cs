using UnityEngine;

namespace Modules.EnemyModule.Data.UnityObjects
{
    /// <summary>How the zombie waves of a zombie match grow, and what one zombie is like.</summary>
    [CreateAssetMenu(fileName = "CD_ZombieWaves", menuName = "Fireline/Enemy/CD_ZombieWaves")]
    public class CD_ZombieWaves : ScriptableObject
    {
        [Header("Waves")]
        [SerializeField] private GameObject _zombiePrefab;
        [SerializeField] private int _firstWaveCount = 3;
        [SerializeField] private int _countIncreasePerWave = 2;
        [SerializeField] private float _firstWaveDelay = 3f;
        [SerializeField] private float _timeBetweenWaves = 8f;

        [Header("Spawning: points closer than the minimum to a player are skipped; with no points, a ring around a player")]
        [SerializeField] private float _spawnRadiusMin = 22f;
        [SerializeField] private float _spawnRadiusMax = 30f;

        [Header("Zombie")]
        [SerializeField] private int _health = 30;
        [SerializeField] private float _moveSpeed = 2.5f;
        [SerializeField] private int _damage = 10;
        [SerializeField] private float _attackRange = 1.3f;
        [SerializeField] private float _attackCooldown = 1f;
        [SerializeField] private float _hitKnockback = 0.4f;

        public GameObject ZombiePrefab => _zombiePrefab;
        public int FirstWaveCount => _firstWaveCount;
        public int CountIncreasePerWave => _countIncreasePerWave;
        public float FirstWaveDelay => _firstWaveDelay;
        public float TimeBetweenWaves => _timeBetweenWaves;
        public float SpawnRadiusMin => _spawnRadiusMin;
        public float SpawnRadiusMax => _spawnRadiusMax;
        public int Health => _health;
        public float MoveSpeed => _moveSpeed;
        public int Damage => _damage;
        public float AttackRange => _attackRange;
        public float AttackCooldown => _attackCooldown;
        public float HitKnockback => _hitKnockback;

        public int CountForWave(int wave) => _firstWaveCount + _countIncreasePerWave * (wave - 1);
    }
}

using System.Collections.Generic;
using Modules.EnemyModule.Data.ValueObjects;
using UnityEngine;

namespace Modules.EnemyModule.Data.UnityObjects
{
    /// <summary>
    /// The zombie match's waves: the kinds of zombie there are, and which wave brings how many of each.
    /// After the last authored wave the last one repeats, a little larger each time.
    /// </summary>
    [CreateAssetMenu(fileName = "CD_ZombieWaves", menuName = "Fireline/Enemy/CD_ZombieWaves")]
    public class CD_ZombieWaves : ScriptableObject
    {
        [Header("Waves")]
        [SerializeField] private GameObject _zombiePrefab;
        [SerializeField] private float _firstWaveDelay = 3f;
        [SerializeField] private float _timeBetweenWaves = 8f;

        [Header("Spawning: points closer than the minimum to a player are skipped; with no points, a ring around a player")]
        [SerializeField] private float _spawnRadiusMin = 22f;
        [SerializeField] private float _spawnRadiusMax = 30f;

        [Header("Zombie kinds")]
        [SerializeField] private List<ZombieTypeCVO> _types = new();

        [Header("Wave table: element 0 is wave 1")]
        [SerializeField] private List<ZombieWaveCVO> _waves = new();
        [Tooltip("Past the table, each extra wave brings this share more of the last wave's zombies (0.25 = +25% per wave).")]
        [SerializeField] private float _growthAfterLastWave = 0.25f;
        [Tooltip("Past the table, every this many waves a boss joins in.")]
        [SerializeField] private int _bossEveryWaves = 5;
        [SerializeField] private string _bossType = "Abomination";

        [Header("Difficulties: Easy, Normal, Nightmare")]
        [SerializeField] private List<ZombieDifficultyCVO> _difficulties = new();

        [Header("Elites")]
        [SerializeField] private ZombieEliteCVO _elite = new();
        [Tooltip("Past the table, the elite chance every group gets, growing this much each extra wave.")]
        [SerializeField] private float _eliteChancePerExtraWave = 0.03f;

        public GameObject ZombiePrefab => _zombiePrefab;
        public float FirstWaveDelay => _firstWaveDelay;
        public float TimeBetweenWaves => _timeBetweenWaves;
        public float SpawnRadiusMin => _spawnRadiusMin;
        public float SpawnRadiusMax => _spawnRadiusMax;
        public IReadOnlyList<ZombieTypeCVO> Types => _types;
        public IReadOnlyList<ZombieWaveCVO> Waves => _waves;
        public float GrowthAfterLastWave => _growthAfterLastWave;
        public int BossEveryWaves => _bossEveryWaves;
        public string BossType => _bossType;
        public ZombieEliteCVO Elite => _elite;
        public ZombieDifficultyCVO Difficulty(int index) =>
            _difficulties.Count == 0 ? new ZombieDifficultyCVO() : _difficulties[Mathf.Clamp(index, 0, _difficulties.Count - 1)];
        public float EliteChancePerExtraWave => _eliteChancePerExtraWave;
    }
}

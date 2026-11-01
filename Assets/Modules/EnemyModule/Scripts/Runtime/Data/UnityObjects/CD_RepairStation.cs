using UnityEngine;

namespace Modules.EnemyModule.Data.UnityObjects
{
    /// <summary>A repair station a player buys: it mends every damaged structure around it a little each second, for free.</summary>
    [CreateAssetMenu(fileName = "CD_RepairStation", menuName = "Fireline/Enemy/CD_RepairStation")]
    public class CD_RepairStation : ScriptableObject
    {
        [SerializeField] private int _health = 300;
        [SerializeField] private float _maxPlaceDistance = 8f;
        [Tooltip("Structures within this distance are mended.")]
        [SerializeField] private float _radius = 8f;
        [Tooltip("Health each structure in range gets back per second.")]
        [SerializeField] private float _healthPerSecond = 12f;

        public int Health => _health;
        public float MaxPlaceDistance => _maxPlaceDistance;
        public float Radius => _radius;
        public float HealthPerSecond => _healthPerSecond;
    }
}

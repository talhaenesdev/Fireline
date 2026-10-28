using UnityEngine;

namespace Modules.EnemyModule.Data.UnityObjects
{
    /// <summary>A turret a player buys: how far it sees, how hard and how often it fires, and how much it takes.</summary>
    [CreateAssetMenu(fileName = "CD_Turret", menuName = "Fireline/Enemy/CD_Turret")]
    public class CD_Turret : ScriptableObject
    {
        [SerializeField] private float _range = 12f;
        [SerializeField] private float _fireInterval = 0.45f;
        [SerializeField] private int _damage = 10;
        [SerializeField] private int _health = 150;
        [Tooltip("How far in front of the buyer it is put down.")]
        [SerializeField] private float _placeDistance = 1.5f;

        public float Range => _range;
        public float FireInterval => _fireInterval;
        public int Damage => _damage;
        public int Health => _health;
        public float PlaceDistance => _placeDistance;
    }
}

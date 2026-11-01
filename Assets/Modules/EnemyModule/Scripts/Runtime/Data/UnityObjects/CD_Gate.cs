using UnityEngine;

namespace Modules.EnemyModule.Data.UnityObjects
{
    /// <summary>A gate a player buys: zombies stop and break it like a wall, players walk through.</summary>
    [CreateAssetMenu(fileName = "CD_Gate", menuName = "Fireline/Enemy/CD_Gate")]
    public class CD_Gate : ScriptableObject
    {
        [SerializeField] private int _health = 450;
        [Tooltip("A zombie this close to a gate stops and breaks it before going on.")]
        [SerializeField] private float _blockRadius = 2f;
        [SerializeField] private float _maxPlaceDistance = 8f;

        public int Health => _health;
        public float BlockRadius => _blockRadius;
        public float MaxPlaceDistance => _maxPlaceDistance;
    }
}
